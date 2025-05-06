using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheStack : MonoBehaviour
{
    private const float BoundSize = 3.5f; //블럭의 크기
    private const float MovingBoundsSize = 3f; //이동량
    private const float StackMovingSpeed = 5.0f; //쌓이는 이동속도
    private const float BlockMovingSpeed = 3.5f; //블록 이동속도
    private const float ErrorMargin = 0.1f; // 성공으로 취급할 기준

    public GameObject originBlock = null; //계속 생성할 블럭 원본

    private Vector3 prevBlockPosition;  //쌓이고 나온 전 블럭 위치
    private Vector3 desiredPosition;   //필요 위치
    private Vector3 stackBounds = new Vector2 (BoundSize, BoundSize); //새로 쌓인 블럭의 사이즈

    Transform lastBlock = null; //최근에 쌓인 블록 트랜스폼
    float blockTransition = 0f;
    float secondaryPosition = 0f;

    int stackCount = -1; //시작해서 +1로 사용할꺼기 때문에 -1로 맞춰놓음 0으로 시작
    public int Score { get {return stackCount; } }
    int comboCount = 0;
    public int Combo { get { return comboCount; } }

    private int maxCombo = 0;
    public int MaxCombo { get => maxCombo; }

    public Color prevColor;
    public Color nextColor;

    bool isMovingx = true;

    int bestScore = 0;
    public int BestScore { get => bestScore; }

    int bestCombo = 0;

    public int BestCombo { get => bestCombo; }

    private const string BestScoreKey = "BestScore";
    private const string BestComboKey = "BestCombo";

    private bool isGameOver = true;

    // Start is called before the first frame update
    void Start()
    {
        if(originBlock == null) //오리진 블럭이 있는지 체크
        {
            Debug.Log("OriginBlock is NULL");
            return;
        }
        
        bestScore = PlayerPrefs.GetInt(BestScoreKey, 0); //게임이 꺼져도 안사라지는 특성
        bestCombo = PlayerPrefs.GetInt (BestComboKey, 0); // 처음 시작할때 점수와 콤보를 가져옴, 널이면 0-디폴트값을 가져옴

        prevColor = GetRandomColor();
        nextColor = GetRandomColor();

        prevBlockPosition = Vector3.down;

        Spawn_Block();
        Spawn_Block();
    }

    // Update is called once per frame
    void Update()
    {
        if (isGameOver) return;

        if(Input.GetMouseButtonDown(0))
        {
            if(PlaceBlock())
            {
                Spawn_Block();

            }
            else
            {
                // 게임 오버
                Debug.Log("Game Over");
                UpdateScore();
                isGameOver = true;
                GameOverEffect();
                UIManager.Instance.SetScoreUI();
            }
        }

        MoveBlock();
        transform.position = Vector3.Lerp(transform.position,desiredPosition,StackMovingSpeed * Time.deltaTime);//TheStack 오브젝트를 부드럽게 이동시키기 위함
    }

    bool Spawn_Block()
    {
        //하단에 오브젝트를 생성해서 사용할꺼기 때문에 월드포지션이 아닌 TheStack안의 로컬 좌표를 사용
        if(lastBlock != null)
        {
            prevBlockPosition = lastBlock.localPosition;
        }

        GameObject newBlock = null;
        Transform newTrans = null;

        newBlock = Instantiate(originBlock);

        if(newBlock == null ) //문제 일어날 가능성은 적지만 예외처리
        {
            Debug.Log("NewBlock Instatantiate Failed");
            return false;
        }
        ColorChange(newBlock);

        newTrans = newBlock.transform; //새롭게 생성된 블럭의 트랜스폼 저장
        newTrans.parent = this.transform; //나 자신을 부모 트랜스폼 지정
        newTrans.localPosition = prevBlockPosition + Vector3.up; //새로운 블럭 (지역)위치값Y를 전 블럭의 y값으로 부터 1올림 
        newTrans.localRotation = Quaternion.identity; //퀀터니언의 초기값, 회전이 없는상태를 저장
        newTrans.localScale = new Vector3(stackBounds.x,1,stackBounds.y);

        stackCount++;

        desiredPosition = Vector3.down * stackCount;//스택 카운트가 증가하는 만큼 TheStack을 Y값을 바닥으로 내림 ,그래야 가장 마지막에 생성되는 블럭이 화면 중앙에 있음
        blockTransition = 0f; //블럭 이동에 대한 기준값 초기화

        lastBlock = newTrans; //마지막 블럭은 새롭게 만들어진 블럭트랜스를 넣어줌

        isMovingx = !isMovingx; //x좌표로 움직이는 토글을 반전

        UIManager.Instance.UpdateScore();
        return true;
    }

    Color GetRandomColor() //rgb값으로 오브젝트 컬러 바꿈
    {
        float r = Random.Range(100f,250f) / 255f; //UI에선 0~255이지만 0~1로 퍼센트 계산 (100의 설정이유는 어두운색 방치)
        float g = Random.Range(100f,250f) / 255f;
        float b = Random.Range(100f,250f) / 255f; 

        return new Color(r, g, b);
    }

    void ColorChange(GameObject go)
    {
        Color applyColor = Color.Lerp(prevColor, nextColor, (stackCount) % 11/10f); //스택의 갯수를 11의나머지 1~10까지의 값을 10을 나눠주면 0~1까지의 값이 나옴
    
        Renderer rn = go.GetComponent<Renderer>(); //renderer은 무언갈 그림

        if (rn == null) //예외처리
        {
            Debug.Log("Renderer is NULL");
            return;
        }

        rn.material.color = applyColor;
        Camera.main.backgroundColor = applyColor - new Color(0.1f,0.1f,0.1f);


        if(applyColor.Equals(nextColor) == true)
        {
            prevColor = nextColor;
            nextColor = GetRandomColor();
        }    
    }


    void MoveBlock()
    {
        blockTransition += Time.deltaTime * BlockMovingSpeed; //1초에 blockMovingSpeed만큼이동

        float movePosition = Mathf.PingPong(blockTransition,BoundSize) - BoundSize / 2; //양수의 범위에서만 왔다갔다할 예정

        if(isMovingx)
        {   //x좌표에서 블럭이 왔다갔다함
            lastBlock.localPosition = new Vector3(
                movePosition *MovingBoundsSize, stackCount, secondaryPosition);
        }
        else
        {   //z좌표에서 블럭이 왔다갔다함
            lastBlock.localPosition = new Vector3(
                secondaryPosition, stackCount, movePosition * MovingBoundsSize);
        }
    }

    bool PlaceBlock()
    {
        Vector3 lastPosition = lastBlock.localPosition;

        if(isMovingx)
        {
            float deltaX = prevBlockPosition.x - lastPosition.x; //쌓인 블럭과 마지막 블럭의 x값 차이
            bool isNegativeNum = (deltaX < 0) ? true : false; //움직이는 방향에 맞춰 러블을 떨어트리기 위함임

            deltaX = Mathf.Abs(deltaX);//델타X의 절대값을 구함

            if(deltaX > ErrorMargin) //실패 오차 범위보다 deltaX가 크면
            {
                stackBounds.x -= deltaX; //생성될 블럭의 X값에 deltaX값을 빼줌
                if(stackBounds.x <= 0)  //만약 생성될 블럭 X값이 0하고 같거나 작다면
                {
                    return false; //게임오버
                }

                float middle = (prevBlockPosition.x + lastPosition.x) / 2; //전에 쌓인 블럭과 마지막 블럭 중심 값을 찾음
                lastBlock.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);// 마지막 블럭의 크기를 바꿔줌

                Vector3 tempPosition = lastBlock.localPosition; //마지막 포지션값 임시 저장
                tempPosition.x = middle; //중심값을 새로 지정해줄 마지막 블럭에 저장
                lastBlock.localPosition = lastPosition = tempPosition; //마지막 블럭 위치 조정

                float rubbleHalfScale = deltaX / 2f;
                CreateRubble(
                    new Vector3(
                        isNegativeNum
                        ? lastPosition.x + stackBounds.x / 2 + rubbleHalfScale  //중심점을 잡는 용도
                        : lastPosition.x - stackBounds.x / 2 - rubbleHalfScale
                        , lastPosition.y
                        , lastPosition.z
                    ),
                    new Vector3(deltaX, 1, stackBounds.y)
                    );

                comboCount = 0;

            }
            else
            {
                ComboCheck();
                lastBlock.localPosition = prevBlockPosition + Vector3.up; //성공했을땐 그대로
            }
        }
        else
        { 
            float deltaZ = prevBlockPosition.z - lastPosition.z;
            bool isNegativeNum = (deltaZ < 0) ? true : false; 

            deltaZ = Mathf.Abs(deltaZ);

            //x축 움직임이 false일때 z축에서 실패할때 크기가 줄어듬 
            if (deltaZ > ErrorMargin) 
            {
                stackBounds.y -= deltaZ;
                if(stackBounds.y <= 0)
                {
                    return false;
                }

                float middle = (prevBlockPosition.z - lastPosition.z)/2 ;
                lastBlock.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);

                Vector3 tempPosition = lastBlock.localPosition;
                tempPosition.z = middle;
                lastBlock.localPosition = lastPosition = tempPosition;

                float rubbleHalfScale = deltaZ / 2;
                CreateRubble(
                    new Vector3(
                        lastPosition.x
                        , lastPosition.y
                        , isNegativeNum
                        ? lastPosition.z + stackBounds.y / 2 + rubbleHalfScale
                        : lastPosition.z - stackBounds.y / 2 - rubbleHalfScale)
                    , new Vector3(stackBounds.x , 1, deltaZ)
                    );

                comboCount = 0;
            }
            else
            {
                ComboCheck();
                lastBlock.localPosition = prevBlockPosition + Vector3.up;
            }
        }

        secondaryPosition = (isMovingx) ? lastBlock.localPosition.x : lastBlock.localPosition.z; //이전위치가 계속해서 바뀌고 있기때문에 0의 위치를 사용x,각각의 위치바뀐걸 기준으로 넣어줌 

        return true;
    }


    void CreateRubble(Vector3 pos, Vector3 scale)
    {
        GameObject go = Instantiate(lastBlock.gameObject); //지금 이동하는 블럭을 가져옴
        go.transform.parent = this.transform;

        go.transform.localPosition = pos;
        go.transform.localScale = scale;
        go.transform.localRotation = Quaternion.identity;

        go.AddComponent<Rigidbody>();
        go.name = "Rubble";
    }


    void ComboCheck()
    {
        comboCount++;

        if(comboCount > maxCombo)
        {
            maxCombo = comboCount;
        }

        if ((comboCount % 5) == 0)
        {
            Debug.Log("5 Combo Success!!");
            stackBounds += new Vector3(0.5f,0.5f);
            stackBounds.x =
                (stackBounds.x > BoundSize) ? BoundSize : stackBounds.x;
            stackBounds.y =
                (stackBounds.y > BoundSize) ? BoundSize : stackBounds.y;
        }
    }

    void UpdateScore()
    {
        if(bestScore < stackCount)
        {
            Debug.Log("최고점수 갱신");
            bestScore = stackCount;
            bestCombo = maxCombo;

            PlayerPrefs.SetInt(BestScoreKey,bestScore);
            PlayerPrefs.SetInt(BestComboKey,bestCombo);
        }
    }

    void GameOverEffect()
    {
        int childCount = this.transform.childCount; //현재 GameObject의 자식 오브젝트가 몇 개인지를 가져와서 childCount라는 변수에 저장한다.

        for(int i = 1; i< 20; i++ )
        {
            if (childCount < i) break;

            GameObject go  = transform.GetChild(childCount - i).gameObject; //하위의 차일드 오브젝트를 인덱스로 찾아옴

            if (go.name.Equals("Rubble")) continue; //러블은 따로 처리하지않음

            Rigidbody rigid = go.AddComponent<Rigidbody>(); // 가져온 하위 오브젝트에 리지드바디 추가

            rigid.AddForce(
                (Vector3.up * Random.Range(0, 10f) + Vector3.right * (Random.Range(0,10f) -5f ))* 100f //확실히 날아갈수 있게
                ); //addForce 힘을 위,오른쪽으로 줘서 오브젝트를 밀어냄
        }
    }

    public void Restart()
    {
        int childCount = transform.childCount;

        for(int  i = 0; i<childCount;i++ )
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        isGameOver = false;

        lastBlock = null;
        desiredPosition = Vector3.zero;
        stackBounds = new Vector3(BoundSize, BoundSize);

        stackCount = -1;
        isMovingx = true;
        blockTransition = 0f;
        secondaryPosition = 0f;

        comboCount = 0;
        maxCombo = 0;

        prevBlockPosition = Vector3.down;

        prevColor = GetRandomColor();
        nextColor = GetRandomColor();

        Spawn_Block();
        Spawn_Block();
    }
}
