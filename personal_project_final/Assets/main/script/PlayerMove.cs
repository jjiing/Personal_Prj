using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerMove : MonoBehaviour
{
    private float turnSpeed; //���콺 ȸ�� �ӵ�
    public float xRotate;   //���� ����� x�� ȸ������ ���� ����(ī�޶� ��/��)

    float walkSpeed;
    float runSpeed;
    float slowSpeed;
    float slowRunSpeed;
    public float currentSpeed;
    float jumpSpeed;
    float gravity;

    private Vector3 moveForce;

    public AudioClip walkSound;
    public AudioClip jumpSound;
    public AudioClip landSound;
   
    public float walkSoundTime;
    public float currentWalkSoundTime;

    CharacterController controller;
    public AudioSource audioSource;

    Vector3 moveDir;

    public bool isRun;
    public bool isWalk;
    public bool isSlow;

    float slowTimer;
    float currentSlowTimer;
    public Image blind;

   public float TurnSpeed
    {
        get {  return turnSpeed; }
        set { turnSpeed = value; }
    }
    public float CurrentSpeed
    {
        get { return currentSpeed; }
        set { currentSpeed = value; }
    }
    public float CurrentSlowTimer
    {
        get { return currentSlowTimer; }
        set { currentSlowTimer = value; }
    }
    public float SlowTimer
    {
        get { return slowTimer; }
    }


    private void Awake()
    {
        
        //���콺 Ŀ�� ������ �ʰ� �����ϰ�, ���� ��ġ�� ������Ų��.
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;


        controller = GetComponent<CharacterController>();
        //audioSource = GetComponent<AudioSource>();


        walkSpeed = 25f;
        runSpeed = walkSpeed *1.5f;
        slowSpeed = walkSpeed * 0.5f;
        slowRunSpeed = walkSpeed;
        currentSpeed = walkSpeed;
        jumpSpeed = 30f;
        gravity = 40;

        currentWalkSoundTime = 0.2f;

        turnSpeed = 0.8f;
        xRotate = 0f;

        isRun = false;
        isWalk = false;
        isSlow = false;

       

        slowTimer = 5f;
        CurrentSlowTimer = 0f;


    }
    
    void Update()
    {
        
        MouseRotation();
        KeyboardMove();
        SlowControl();
           

    }
    void SlowControl()
    {
        if (CurrentSlowTimer > 0)
        {
            Slow();
            isSlow = true;
        }
        else
        {
            currentSpeed = walkSpeed;
            isSlow = false;
            runSpeed = slowRunSpeed * 1.5f ;
        }
    }
    void Slow()
    {
        CurrentSlowTimer -= Time.deltaTime;
        currentSpeed = slowSpeed;
        runSpeed = slowRunSpeed;

    }

    private void MouseRotation()
    {

        float yRotateSize = Input.GetAxis("Mouse X") * turnSpeed;
        // �¿�� ������ ���콺�� �̵��� * �ӵ��� ���� ī�޶� �¿�� ȸ���� �� ���
        float yRotate = transform.eulerAngles.y + yRotateSize;
        // ���� y�� ȸ������ ���� ���ο� ȸ������ ���

        float xRotateSize = -Input.GetAxis("Mouse Y") * turnSpeed;
        // ���Ʒ��� ������ ���콺�� �̵��� * �ӵ��� ���� ī�޶� ȸ���� �� ���(�ϴ�, �ٴ��� �ٶ󺸴� ����)
        xRotate = Mathf.Clamp(xRotate + xRotateSize, -45, 80);
        // ���Ʒ� ȸ������ ���������� -45�� ~ 80���� ���� (-45:�ϴù���, 80:�ٴڹ���)
        // ȸ�� ���� ���� (ȭ�� ������ ���콺�� �������� x�� ȸ�� ���� �ϵ� ��� ���� ���� ����)
        // Clamp �� ���� ������ �����ϴ� �Լ�

        transform.eulerAngles = new Vector3(xRotate, yRotate, 0);
        // ī�޶� ȸ������ ī�޶� �ݿ�(X, Y�ุ ȸ��)


    }

    private void KeyboardMove()
    {
        //�������� �� �� �ְ� ����
        if (isWalk)
            CheckRun();
        else
            isRun = false;


        //�̵�(x,z��)
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 direction = transform.rotation * new Vector3(x, 0, z);
        //�̵� ���� = ĳ������ ȸ�� �� * ���� ��
        //ī�޶� ȸ������ ���� ������ ���ؾ� �ϱ� ������ ȸ�� ���� ���Ѵ�.

        moveForce = new Vector3(direction.x * currentSpeed, moveDir.y, direction.z * currentSpeed);
        //�̵� �� = �̵����� * �ӵ�


        controller.Move(moveForce * Time.deltaTime);
        //1�ʴ� moveForce �ӷ����� �̵�


        //y�� ���� : �߷�, ����
        if (controller.isGrounded)
        {
            if (Input.GetButton("Jump"))
            {
                moveDir.y = jumpSpeed;
                StartCoroutine(JumpSoundCo());
            }
        }
        moveDir.y -= gravity * Time.deltaTime;
        // �߷��� ������ �޾� �Ʒ������� �ϰ�


        //isWalk ��Ʈ��
        if (Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
            isWalk = true;

        else
            isWalk = false;

        if(!audioSource.isPlaying&& controller.isGrounded)
            WalkSound();
    }
    private void CheckRun()
    {
        if (Input.GetKey(KeyCode.LeftShift))
            Running(true);
        if (Input.GetKeyUp(KeyCode.LeftShift))
            Running(false); //running cancel
    }
    private void Running(bool run)
    {
        isRun = run;

        if (run)
            currentSpeed = runSpeed;
        else
            currentSpeed = walkSpeed;
    }
    private void PlaySE(AudioClip _clip)
    {
        audioSource.clip = _clip;
        audioSource.Play();
    }

    private void PlayDelaySE(AudioClip _clip)
    {
        audioSource.clip = _clip;
        audioSource.PlayDelayed(0.1f);
    }
    private void PlayDelay2SE(AudioClip _clip)
    {
        audioSource.clip = _clip;
        audioSource.PlayDelayed(0.25f);
    }
    private void StopSE(AudioClip _clip)
    {
        audioSource.clip = _clip;
        audioSource.Stop();
    }
    IEnumerator JumpSoundCo()
    {
        PlaySE(jumpSound);
        yield return new WaitForSeconds(1.5f);
        PlaySE(landSound);
    }
    private void WalkSound()
    {
        if (!isSlow && isWalk && isRun)
            PlaySE(walkSound);
        else if (!isSlow && isWalk && !isRun || isSlow && isWalk && isRun)
            PlayDelaySE(walkSound);
        else if (isSlow && isWalk && !isRun)
            PlayDelay2SE(walkSound);
        else
            StopSE(walkSound);

    }

  public void   Blind()
    {
        blind.gameObject.SetActive(true);
        Invoke("BlindOff", 2.5f);
    }

    private void BlindOff()
    {
        blind.gameObject.SetActive(false);
    }



}
