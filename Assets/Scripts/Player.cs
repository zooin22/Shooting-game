using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Player : Charcter
{
    List<Weapon> weapons;
    Weapon weapon;

    private void Start()
    {
        speed = 2f;
        state = State.IDLE;
        weapons = new List<Weapon>();
        weapons.Add(new NoWeapon(0,0,0,0,0,0,0));
        weapons.Add(new BaseGun  (5, 50, 10, 1, 5, 1, 10));
        weapons.Add(new BounceGun(5, 50, 10, 1, 5, 1, 10, 2));
        weapons.Add(new ShotGun  (5, 50, 10, 1, 5, 2, 10));
        weapons.Add(new LaserGun (5, 50, 1000, 1, 10, 0, 10));
        weapon = weapons[0];
    }
    public void SwapWeapon(int i)
    {
        if (i >= weapons.Count)
            return;
        weapon.Stop(transform, Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0)));
        weapon = weapons[i];
    }

    private void Roll(Vector3 direction)
    {
        StartCoroutine(RollRoutine(direction));
    } // 구르기

    IEnumerator RollRoutine(Vector3 direction) 
    {
        state = State.ROLL; // 구르기 상태 
        for (int i = 0; i < 10 ;i++) // 10프레임동안 움직이던 방향으로 구르기
        {
            transform.position = transform.position + 2 * direction;
            yield return new WaitForEndOfFrame();
        }
        state = State.IDLE; // Idle 상태 회복
    }
#region override
    protected override void Move()
    {
        if (State.ROLL == state) // 구르고 있을 시 못 움직임.
            return;
        oldPos = transform.position;
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
            Roll(transform.position - oldPos);
            weapon.Stop(transform, Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0)));
            return;
        }
        if (oldPos == transform.position) // 움직이지 않을 시 
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
        if (Input.GetKey(KeyCode.Alpha1))
            SwapWeapon(0);
        if (Input.GetKey(KeyCode.Alpha2))
            SwapWeapon(1);
        if (Input.GetKey(KeyCode.Alpha3))
            SwapWeapon(2);
        if (Input.GetKey(KeyCode.Alpha4))
            SwapWeapon(3);
        if (Input.GetKey(KeyCode.Alpha5))
            SwapWeapon(4);

        if (Input.GetKey(KeyCode.R))
            weapon.Reload();
    }
    protected override void Shot() // 미사일 발사
    {
        if (State.ROLL == state) // 구르고 있을 시 발사 안됨.
            return;
        if (Input.GetMouseButton(0)) // 왼쪽 클릭
        {
            weapon.MouseDown(this.transform, Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,Input.mousePosition.y,0))); // Weapon에 현재 위치, 목표 위치 전달
        }
        if (Input.GetMouseButtonUp(0)) // 왼쪽 클릭
        {
            weapon.MouseUp(this.transform, Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0))); // Weapon에 현재 위치, 목표 위치 전달
        }
    }
#endregion
    private void Update()
    {
        Move();
        Action();
        Shot();
    }
}
