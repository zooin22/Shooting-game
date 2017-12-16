using UnityEngine;

class BulletWrapper : BaseObject // cBullet 클래스가 Decorator Pattern이라 new 생성자를 사용하는데 Unity의 경우 new 키워드로 인한 생성자는 null을 반납함으로 이를 보완하기 위한 래핑 클래스의 일종
{
    Bullet bullet;

    public void SetBullet(Bullet bullet, Vector3 direction,Vector3 shotPos) // 총알 속성 초기화
    {
        this.bullet = bullet; // 총알 속성 스크립트
        this.bullet.SetBullet(direction, shotPos); // 총알 방향 초기화
    }
    
    private void OnCollisionEnter2D(Collision2D coll) // 충돌 시
    {
        if (null == this.bullet || coll.transform.CompareTag("Player") || coll.transform.CompareTag("Bullet"))// 플레이어 or bullet에는 충돌하지 않음
            return;
        this.bullet.OnCollisionEnter2D(coll);
    }

    private void FixedUpdate()// 이동 루틴
    {

        if (null == this.bullet)
            return;
        this.bullet.Update();
    }

    public void Destroy() //삭제 
    {
        if (null == this.bullet)
            return;
        Destroy(this);
    }
}