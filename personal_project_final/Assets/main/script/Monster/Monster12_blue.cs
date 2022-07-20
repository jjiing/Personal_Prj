using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster12_blue : Monster
{
    public GameObject target;
    NavMeshAgent nav;
    public Animator anim;
    public GameObject lazerEffect;
    Rigidbody rigidbody;

    
    float attackRate;
    float currentAttackRate;
    int atkDis;
    int detectDis;
    


    

    void Start()
    {
        Hp = 180;
        Atk = 20; //��ġ�� 30. ���������� ������ �¾Ƶ� 1�ʿ� 30��
        atkDis = 50;
        detectDis = 100;

        nav = GetComponent<NavMeshAgent>();
        target = GameObject.FindGameObjectWithTag("Player");
        rigidbody = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        attackRate = 7f;
        currentAttackRate = 0;
        nav.speed = 30;
        nav.stoppingDistance = atkDis;

        monsterCount++;
    }


    void Update()
    {

        Detect();
        DamageAni();

    }
    private void Detect()
    {
        float dis = Vector3.Distance(transform.position, target.transform.position);
        if (dis <= detectDis)
        {
            if (atkDis <= dis)
            {
                MoveToPlayer();
                anim.SetBool("isMove", true);

            }
            else if (dis < atkDis && !isDead)
            {
                //�̲����� - ����
                rigidbody.velocity = Vector3.zero;
                rigidbody.angularVelocity = Vector3.zero;
                nav.velocity = Vector3.zero;

                AttackPlayer();
                anim.SetBool("isMove", false);
            }

            //���� (�����߿� �÷��̾�� �ѹ��� ������ �Ĵٺ���)
            if (!lazerEffect.activeInHierarchy)
                transform.LookAt(target.transform.position - new Vector3(0, 13, 0));
            else
            {
                Vector3 dir = target.transform.position - transform.position;
                var nextRot = Quaternion.LookRotation(dir);

                transform.rotation = Quaternion.Slerp(transform.rotation, nextRot, Time.deltaTime * 0.7f);
            }
        }
        else if (IsDamaged)
        {
            transform.LookAt(target.transform.position);
            MoveToPlayer();
            anim.SetBool("isMove", true);
        }
    }
    
   
    private void MoveToPlayer()
    {

        nav.SetDestination(target.transform.position);
    }
    public override void AttackPlayer()
    {

        
        if (currentAttackRate > 0)
            currentAttackRate -= Time.deltaTime;

        //�������� �������� �־��
        if (currentAttackRate <= 0)
        {
            currentAttackRate = attackRate;
            lazerEffect.SetActive(true);
            Invoke("DeactiveLazer", 5f);
            currentAttackRate = attackRate;
            
        }

    }
    private void DeactiveLazer()
    {
        lazerEffect.SetActive(false);
    }
    public override void MonsterDie()
    {
        anim.SetTrigger("isDie");
        Destroy(gameObject, 1.5f); //�״� �ִϸ��̼� �� �ı�
        isDead = true;
        DropHeal();
    }
    private void DamageAni()
    {
        if (isDamaged)
        {
            anim.SetTrigger("isDamaged");
            isDamaged = false;
        }
    }



}
