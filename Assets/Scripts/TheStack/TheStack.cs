using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheStack : MonoBehaviour
{
    private const float BoundSize = 3.5f; //���� ũ��
    private const float MovingBoundsSize = 3f; //�̵���
    private const float StackMovingSpeed = 5.0f; //���̴� �̵��ӵ�
    private const float BlockMovingSpeed = 3.5f; //��� �̵��ӵ�
    private const float ErrorMargin = 0.1f; // �������� ����� ����

    public GameObject originBlock = null; //��� ������ �� ����

    private Vector3 prevBlockPosition;  //���̰� ���� �� �� ��ġ
    private Vector3 desiredPosition;   //�ʿ� ��ġ
    private Vector3 stackBounds = new Vector2 (BoundSize, BoundSize); //���� ���� ���� ������

    Transform lastBlock = null; //�ֱٿ� ���� ��� Ʈ������
    float blockTransition = 0f;
    float secondaryPosition = 0f;

    int stackCount = -1; //�����ؼ� +1�� ����Ҳ��� ������ -1�� ������� 0���� ����
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
        if(originBlock == null) //������ ���� �ִ��� üũ
        {
            Debug.Log("OriginBlock is NULL");
            return;
        }
        
        bestScore = PlayerPrefs.GetInt(BestScoreKey, 0); //������ ������ �Ȼ������ Ư��
        bestCombo = PlayerPrefs.GetInt (BestComboKey, 0); // ó�� �����Ҷ� ������ �޺��� ������, ���̸� 0-����Ʈ���� ������

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
                // ���� ����
                Debug.Log("Game Over");
                UpdateScore();
                isGameOver = true;
                GameOverEffect();
                UIManager.Instance.SetScoreUI();
            }
        }

        MoveBlock();
        transform.position = Vector3.Lerp(transform.position,desiredPosition,StackMovingSpeed * Time.deltaTime);//TheStack ������Ʈ�� �ε巴�� �̵���Ű�� ����
    }

    bool Spawn_Block()
    {
        //�ϴܿ� ������Ʈ�� �����ؼ� ����Ҳ��� ������ ������������ �ƴ� TheStack���� ���� ��ǥ�� ���
        if(lastBlock != null)
        {
            prevBlockPosition = lastBlock.localPosition;
        }

        GameObject newBlock = null;
        Transform newTrans = null;

        newBlock = Instantiate(originBlock);

        if(newBlock == null ) //���� �Ͼ ���ɼ��� ������ ����ó��
        {
            Debug.Log("NewBlock Instatantiate Failed");
            return false;
        }
        ColorChange(newBlock);

        newTrans = newBlock.transform; //���Ӱ� ������ ���� Ʈ������ ����
        newTrans.parent = this.transform; //�� �ڽ��� �θ� Ʈ������ ����
        newTrans.localPosition = prevBlockPosition + Vector3.up; //���ο� �� (����)��ġ��Y�� �� ���� y������ ���� 1�ø� 
        newTrans.localRotation = Quaternion.identity; //���ʹϾ��� �ʱⰪ, ȸ���� ���»��¸� ����
        newTrans.localScale = new Vector3(stackBounds.x,1,stackBounds.y);

        stackCount++;

        desiredPosition = Vector3.down * stackCount;//���� ī��Ʈ�� �����ϴ� ��ŭ TheStack�� Y���� �ٴ����� ���� ,�׷��� ���� �������� �����Ǵ� ���� ȭ�� �߾ӿ� ����
        blockTransition = 0f; //�� �̵��� ���� ���ذ� �ʱ�ȭ

        lastBlock = newTrans; //������ ���� ���Ӱ� ������� ��Ʈ������ �־���

        isMovingx = !isMovingx; //x��ǥ�� �����̴� ����� ����

        UIManager.Instance.UpdateScore();
        return true;
    }

    Color GetRandomColor() //rgb������ ������Ʈ �÷� �ٲ�
    {
        float r = Random.Range(100f,250f) / 255f; //UI���� 0~255������ 0~1�� �ۼ�Ʈ ��� (100�� ���������� ��ο�� ��ġ)
        float g = Random.Range(100f,250f) / 255f;
        float b = Random.Range(100f,250f) / 255f; 

        return new Color(r, g, b);
    }

    void ColorChange(GameObject go)
    {
        Color applyColor = Color.Lerp(prevColor, nextColor, (stackCount) % 11/10f); //������ ������ 11�ǳ����� 1~10������ ���� 10�� �����ָ� 0~1������ ���� ����
    
        Renderer rn = go.GetComponent<Renderer>(); //renderer�� ���� �׸�

        if (rn == null) //����ó��
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
        blockTransition += Time.deltaTime * BlockMovingSpeed; //1�ʿ� blockMovingSpeed��ŭ�̵�

        float movePosition = Mathf.PingPong(blockTransition,BoundSize) - BoundSize / 2; //����� ���������� �Դٰ����� ����

        if(isMovingx)
        {   //x��ǥ���� ���� �Դٰ�����
            lastBlock.localPosition = new Vector3(
                movePosition *MovingBoundsSize, stackCount, secondaryPosition);
        }
        else
        {   //z��ǥ���� ���� �Դٰ�����
            lastBlock.localPosition = new Vector3(
                secondaryPosition, stackCount, movePosition * MovingBoundsSize);
        }
    }

    bool PlaceBlock()
    {
        Vector3 lastPosition = lastBlock.localPosition;

        if(isMovingx)
        {
            float deltaX = prevBlockPosition.x - lastPosition.x; //���� ���� ������ ���� x�� ����
            bool isNegativeNum = (deltaX < 0) ? true : false; //�����̴� ���⿡ ���� ������ ����Ʈ���� ������

            deltaX = Mathf.Abs(deltaX);//��ŸX�� ���밪�� ����

            if(deltaX > ErrorMargin) //���� ���� �������� deltaX�� ũ��
            {
                stackBounds.x -= deltaX; //������ ���� X���� deltaX���� ����
                if(stackBounds.x <= 0)  //���� ������ �� X���� 0�ϰ� ���ų� �۴ٸ�
                {
                    return false; //���ӿ���
                }

                float middle = (prevBlockPosition.x + lastPosition.x) / 2; //���� ���� ���� ������ �� �߽� ���� ã��
                lastBlock.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);// ������ ���� ũ�⸦ �ٲ���

                Vector3 tempPosition = lastBlock.localPosition; //������ �����ǰ� �ӽ� ����
                tempPosition.x = middle; //�߽ɰ��� ���� �������� ������ ���� ����
                lastBlock.localPosition = lastPosition = tempPosition; //������ �� ��ġ ����

                float rubbleHalfScale = deltaX / 2f;
                CreateRubble(
                    new Vector3(
                        isNegativeNum
                        ? lastPosition.x + stackBounds.x / 2 + rubbleHalfScale  //�߽����� ��� �뵵
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
                lastBlock.localPosition = prevBlockPosition + Vector3.up; //���������� �״��
            }
        }
        else
        { 
            float deltaZ = prevBlockPosition.z - lastPosition.z;
            bool isNegativeNum = (deltaZ < 0) ? true : false; 

            deltaZ = Mathf.Abs(deltaZ);

            //x�� �������� false�϶� z�࿡�� �����Ҷ� ũ�Ⱑ �پ�� 
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

        secondaryPosition = (isMovingx) ? lastBlock.localPosition.x : lastBlock.localPosition.z; //������ġ�� ����ؼ� �ٲ�� �ֱ⶧���� 0�� ��ġ�� ���x,������ ��ġ�ٲ�� �������� �־��� 

        return true;
    }


    void CreateRubble(Vector3 pos, Vector3 scale)
    {
        GameObject go = Instantiate(lastBlock.gameObject); //���� �̵��ϴ� ���� ������
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
            Debug.Log("�ְ����� ����");
            bestScore = stackCount;
            bestCombo = maxCombo;

            PlayerPrefs.SetInt(BestScoreKey,bestScore);
            PlayerPrefs.SetInt(BestComboKey,bestCombo);
        }
    }

    void GameOverEffect()
    {
        int childCount = this.transform.childCount; //���� GameObject�� �ڽ� ������Ʈ�� �� �������� �����ͼ� childCount��� ������ �����Ѵ�.

        for(int i = 1; i< 20; i++ )
        {
            if (childCount < i) break;

            GameObject go  = transform.GetChild(childCount - i).gameObject; //������ ���ϵ� ������Ʈ�� �ε����� ã�ƿ�

            if (go.name.Equals("Rubble")) continue; //������ ���� ó����������

            Rigidbody rigid = go.AddComponent<Rigidbody>(); // ������ ���� ������Ʈ�� ������ٵ� �߰�

            rigid.AddForce(
                (Vector3.up * Random.Range(0, 10f) + Vector3.right * (Random.Range(0,10f) -5f ))* 100f //Ȯ���� ���ư��� �ְ�
                ); //addForce ���� ��,���������� �༭ ������Ʈ�� �о
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
