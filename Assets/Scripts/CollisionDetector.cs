using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CollisionDetector : MonoBehaviour
{
    private static readonly bool isVisible = false;
    private Collider2D col;
    private bool isColliderEnabled;
    public bool IsColliderEnabled
    {
        get => isColliderEnabled;
        set
        {
            isColliderEnabled = value;
            col.enabled = value;
        }
    }
    public Entity Owner { get; private set; }
    public event System.Action<Collision2D> OnCollisionEnter;
    public event System.Action<Collision2D> OnCollisionExit;
    public event System.Action<Collider2D> OnTriggerEnter;
    public event System.Action<Collider2D> OnTriggerExit;
    private void Awake()
    {
        col = GetComponent<Collider2D>();
        if (isVisible && TryGetComponent<SpriteRenderer>(out var sprite)) sprite.enabled = true;
    }
    public void SetOwner(Entity entity) => Owner = entity;
    private void OnCollisionEnter2D(Collision2D collision) => OnCollisionEnter?.Invoke(collision);
    private void OnCollisionExit2D(Collision2D collision) => OnCollisionExit?.Invoke(collision);
    private void OnTriggerEnter2D(Collider2D collision) => OnTriggerEnter?.Invoke(collision);
    private void OnTriggerExit2D(Collider2D collision) => OnTriggerExit?.Invoke(collision);
}
