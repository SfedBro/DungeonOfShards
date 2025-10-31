using System;

[Serializable]
public class BaseShard
{
    public virtual void Update() {}

    public virtual void OnHit(Enemy enemy) {}
}
