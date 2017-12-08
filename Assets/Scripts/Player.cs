using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Player : Charcter
{
    WeaponBag weaponBag;
    Weapon weapon;

    private void Start()
    {
        speed = 2f;
        state = State.IDLE;
        weaponBag = new WeaponBag(gameObject);
        weaponBag.Add(new BaseGun  (5, 50, 10, 3, 10, 0.1f, 10));
        weaponBag.Add(new BounceGun(5, 50, 10, 1, 10, 1, 10, 2));
        weaponBag.Add(new ShotGun  (5, 50, 10, 1, 10, 2, 10));
        weaponBag.Add(new LaserGun(5, 50, 10, 1, 100, 2, 10));
        weapon = weaponBag.Init();
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
        if (Input.GetKeyDown(KeyCode.Alpha1))
            weaponBag.SwapWeapon(ref weapon, 0);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            weaponBag.SwapWeapon(ref weapon, 1);
        if (Input.GetKeyDown(KeyCode.Alpha3))
            weaponBag.SwapWeapon(ref weapon, 2);
        if (Input.GetKeyDown(KeyCode.Alpha4))
            weaponBag.SwapWeapon(ref weapon, 3);
        if (Input.GetKeyDown(KeyCode.Alpha5))
            weaponBag.SwapWeapon(ref weapon, 4);

        if (Input.GetKeyDown(KeyCode.R))
            weapon.Reload();
    }
    protected override void Shot() // 미사일 발사
    {
        if (State.ROLL == state || weapon.GetGunState() == GunState.RELOAD) // 구르고 있을 시, 재장전 시 발사 안됨.
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
