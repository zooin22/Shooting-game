using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Charcter
{
    public Transform arm;
    public SpriteRenderer gunBody;
    public Transform gunBarrel;
    Vector3 aim;

    WeaponBag weaponBag;
    Weapon weapon;

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
   
    private void Raycast()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.left, 0.1f);
    }
    public Weapon GetWeapon() { return weapon; }
    public void SetWeapon(Weapon weapon) { this.weapon = weapon; gunBody.sprite = weapon.GetSprite(); }
    #region override
    protected override void Aiming()
    {
        aim = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
        arm.rotation = BulletCal.GetRotFromVector(aim - this.transform.position);
        arm.Rotate(new Vector3(0, 0, -90));
    } 
    protected override void Attacked(int damage)
    {
        this.hp -= damage;
    }
    protected override void Move()
    {
        if (State.ROLL == state) // 구르고 있을 시 못 움직임.
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
        throw new System.NotImplementedException();
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
