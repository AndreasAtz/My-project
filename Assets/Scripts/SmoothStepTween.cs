using System.Collections;
using UnityEngine;

public class SmoothStepTween : MonoBehaviour
{
    [ SerializeField ] private float speed;
    [ SerializeField ] private float waitTime;
    [ SerializeField ] private Vector3 startPosition;
    [ SerializeField ] private Vector3 endPosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private IEnumerator SmoothStepForward() {
        float t = 0.0f;
        while (t < 1.0f) {
            float g = Mathf .SmoothStep(0.0f, 1.0f, t);
            this .transform.position = Vector3 .Lerp( this .startPosition, this .endPosition, g);
            t += Time .deltaTime * this .speed;
            yield return null ;
        }
        this .transform.position = this .endPosition;
    }
    private IEnumerator SmoothStepBackward() {
        float t = 1.0f;
        while (t > 0.0f) {
            float g = Mathf .SmoothStep(0.0f, 1.0f, t);
            this .transform.position = Vector3 .Lerp( this .startPosition, this .endPosition, g);
            t -= Time .deltaTime * this .speed;
            yield return null ;
        }
            this .transform.position = this .startPosition;
    }
        private IEnumerator SmoothMove() {
            yield return new WaitForSeconds ( this .waitTime);
            yield return this .StartCoroutine( this .SmoothStepForward());
            yield return new WaitForSeconds ( this .waitTime);
            yield return this .StartCoroutine( this .SmoothStepBackward());
        }
        private void Start () 
        {
            this.transform.position = this.startPosition;
            this .StartCoroutine( this .SmoothMove());
        }
    }




