using System.Collections;
using UnityEngine;

public class CurveAnimation : MonoBehaviour {
    [ SerializeField ] private float speed = 0.5f;
    [ SerializeField ] private float waitTime = 1f;
    [ SerializeField ] private Vector3 startPosition;
    [ SerializeField ] private Vector3 endPosition;
    [ SerializeField ] private AnimationCurve curve;
    private IEnumerator Animate()
    {
        float t = 0.0f;
        while (t < 1.0f)
        {
            var position = Vector3 .Lerp( this .startPosition, this .endPosition, t);
            position.y += this .curve.Evaluate(t);
            this .transform.position = position;
            t += Time .deltaTime * this .speed;
            yield return null ;
        }
        this .transform.position = this .endPosition + Vector3 .up * this. curve.Evaluate(1);
    }
    private void Start () {
        this .StartCoroutine( this .Animate());
    }
}
