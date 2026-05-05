using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG;
public class TweenedMovingPlatform : MonoBehaviour
{
    [SerializeField] private List<Vector3> positions;
    [SerializeField] private List<float> movingTimes;
    [SerializeField] private float waitingTime;
    private bool isPlaying = false;
    private Sequence sequence;
    private void CreateSequence() 
    {
        this.sequence = DOTween.Sequence();
        for(int i = 0; i < this.positions.Count; i++) 
        {
            var tween = this.transform.DOMove(this.positions[i], this.movingTimes[i]);
            tween.SetEase(Ease.InOutQuint);
            this.sequence.Append(tween);
            this.sequence.AppendInterval(this.waitingTime);
        }
        for(int i = this.positions.Count - 2; i >= 1; i--) 
        {
            var tween = this.transform.DOMove(this.positions[i], this.movingTimes[i]);
            tween.SetEase(Ease.InOutQuint);
            this.sequence.Append(tween);
            this.sequence.AppendInterval(this.waitingTime);
        }
    }
    IEnumerator Play()
    {
        this.isPlaying = true;
        this.CreateSequence();
        this.sequence.Play();
        yield return this.sequence.WaitForCompletion();
        this.isPlaying = false;
    }
    void Update()
    {
        if(!this.isPlaying) 
        {
            this.StartCoroutine(this.Play());
            this.isPlaying = true;
        }
    }
}