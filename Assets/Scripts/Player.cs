using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Charcter
{
    private float rollCooldown;

    private void Roll(Vector3 direction)
    {
        StartCoroutine(RollRoutine(direction));
    } // 구르기
    IEnumerator RollRoutine(Vector3 direction) 
    {
        state = State.DASH; // 구르기 상태 
        for (int i = 0; i < 10 ;i++) // 10프레임동안 움직이던 방향으로 구르기
        {
            rb.velocity = direction*speed*3;
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
    private IEnumerator KnockBack(Vector2 pushDirection, float knockPower, float knockDur) //일단 enemy랑 중복인데 수정할지도 모름
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
        if (State.DASH == state || State.KNOCKBACK == state) // 넉백, 구르고 있을 시 못 움직임.
            return;
        Vector3 movement = Vector3.zero;

        if (Input.GetKey(KeyCode.A))
            movement += Vector3Int.left;
        if (Input.GetKey(KeyCode.D))
            movement += Vector3Int.right;
        if (Input.GetKey(KeyCode.W))
            movement += Vector3Int.up;
        if (Input.GetKey(KeyCode.S))
            movement += Vector3Int.down;

        rb.velocity = Vector3.ClampMagnitude(movement, 1f) * speed;

        if (movement.magnitude > 0f)
        {
            aim = movement;
        }

        rollCooldown = Mathf.MoveTowards(rollCooldown, 0f, Time.deltaTime);

        if (Input.GetButtonDown("Jump")) // 구르기
        {
            if (rollCooldown <= 0f && movement.magnitude > 0)
            {
                Roll(aim);
                rollCooldown = 1f;
                return;
            }
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
        if (State.DASH == state || weapon.GetGunState() == GunState.RELOAD) // 구르고 있을 시, 재장전 시 발사 안됨.
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
    private void Start()
    {
        speed = 3f;
        state = State.IDLE;
        weaponBag = new WeaponBag(this);
        weapon = weaponBag.Init();
        rb = GetComponent<Rigidbody2D>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Item"))
        {
            Weapon item = other.GetComponent<WeaponItemWrapper>().PickItem() as Weapon;
            weaponBag.Add(item);
            Destroy(other.gameObject);
        }
    }
    private void Update()
    {
        Aiming();
        Shot();
        Action();
    }
    private void FixedUpdate()
    {     
        Move();
    }
    #endregion
}
