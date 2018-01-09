using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Charcter
{
    private void Roll(Vector3 direction)
    {
        StartCoroutine(RollRoutine(direction));
    } // 구르기
    IEnumerator RollRoutine(Vector3 direction) 
    {
        state = State.ROLL; // 구르기 상태 
        for (int i = 0; i < 10 ;i++) // 10프레임동안 움직이던 방향으로 구르기
        {
            transform.position = transform.position + direction*0.1f;
            yield return new WaitForEndOfFrame();
        }
        state = State.IDLE; // Idle 상태 회복
    }
    private void Wheel()
    {
        float d = Input.GetAxis("Mouse ScrollWheel");
        if (d > 0f)
        {
            weaponBag.WheelWeapon(true);
        }
        else if(d < -0f)
        {
            weaponBag.WheelWeapon(false);
        }
    }
    private IEnumerator KnockBack(Vector2 pushDirection, float knockPower, float knockDur)
    {
        this.state = State.KNOCKBACK;
        float timer = 0;
        while (knockDur > timer)
        {
            timer += Time.deltaTime;
            this.GetComponent<Rigidbody2D>().MovePosition(Vector2.MoveTowards(transform.position, new Vector3(transform.position.x + pushDirection.x, transform.position.y + pushDirection.y,0), knockPower * Time.deltaTime)); // the knock back
            yield return new WaitForFixedUpdate();
        }
        this.state = State.IDLE;
    }
    #region override
    public override void Attacked(int damage,Vector2 pushDirection,float knockPower)
    {
        this.hp -= damage;
        StartCoroutine(KnockBack(pushDirection, knockPower, 0.1f));
        if (hp < 0)
            Death();
    }
    protected override void Aiming()
    {
        aim = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
        arm.rotation = BulletCal.GetRotFromVector(aim - this.transform.position);
        arm.Rotate(new Vector3(0, 0, -90));
    } 
    protected override void Move()
    {
        if (State.ROLL == state || State.KNOCKBACK == state) // 넉백, 구르고 있을 시 못 움직임.
            return;
        lastPosition = transform.position;
        if (Input.GetKey(KeyCode.W))
            transform.Translate(Vector3.up * speed * Time.deltaTime);
        if (Input.GetKey(KeyCode.A))
            transform.Translate(Vector3.left * speed * Time.deltaTime);
        if (Input.GetKey(KeyCode.D))
            transform.Translate(Vector3.right * speed * Time.deltaTime);
        if (Input.GetKey(KeyCode.S))
            transform.Translate(Vector3.down * speed * Time.deltaTime);
        if (Input.GetKeyDown(KeyCode.Space)) // 구르기
        {
            if (state == State.WALK)
                Roll((transform.position - lastPosition).normalized);
            return;
        }
        if (lastPosition == transform.position) // 움직이지 않을 시 
        {
            state = State.IDLE;
        }
        else
        {
            state = State.WALK;
        }
    } 
    protected override void Action()
    {
        if (Input.GetKeyDown(KeyCode.R))
            weapon.Reload();
        Wheel();
    } 
    protected override void Shot() 
    {
        if (State.ROLL == state || weapon.GetGunState() == GunState.RELOAD) // 구르고 있을 시, 재장전 시 발사 안됨.
            return;
        if (Input.GetMouseButton(0)) // 왼쪽 다운
        {
            weapon.MouseDown(gunBarrel, Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y))); // Weapon에 현재 위치, 목표 위치 전달
        }
        if (Input.GetMouseButtonUp(0)) // 왼쪽 업
        {
            weapon.MouseUp(gunBarrel, Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y))); // Weapon에 현재 위치, 목표 위치 전달
        }
    }
    protected override void Death()
    {
        Debug.Log("DeatH");
    }
    #endregion
    #region UnityFunction
    private void Awake()
    {
        speed = 2f;
        state = State.IDLE;
        isAlive = true;
        weaponBag = new WeaponBag(this);
        weapon = weaponBag.Init();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Item"))
        {
            Weapon item = collision.GetComponent<WeaponItemWrapper>().PickItem() as Weapon;
            weaponBag.Add(item);
            Destroy(collision.gameObject);
        }
    }
    private void Update()
    {
        Aiming();
        Shot();
    }
    private void FixedUpdate()
    {
        Move();
        Action();
    }
    #endregion
}
