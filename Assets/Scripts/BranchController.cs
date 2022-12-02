using UnityEngine;
using System.Collections;

public class BranchController : MonoBehaviour {

    // Use this for initialization
    public float timeScale = 1f;
    public float timeToWaitToScaleAfterBranchVisible = 0.2f;

    private GameObject temp;
    private Vector3 startScale;
    private Vector3 endScale;
    private bool disableCheck = false;
    private float lengthToScale;

    void Start () {

        temp = GameObject.FindGameObjectWithTag("TrunkManager");

        lengthToScale = Random.Range(1, temp.GetComponent<GameManager>().maxBranchScale);

        startScale = gameObject.transform.parent.localScale;
        endScale = gameObject.transform.parent.localScale + new Vector3(0, 0, lengthToScale);
    }


    void OnBecameVisible()
    {
        if (!disableCheck)
        {
            StartCoroutine(ScaleBranch(startScale, endScale, timeScale));
            disableCheck = true;
        }
    }


    //Scale branch
    IEnumerator ScaleBranch(Vector3 startScale, Vector3 endScale, float time)
    {
        yield return new WaitForSeconds(timeToWaitToScaleAfterBranchVisible);
        float t = 0;
        while (t < time)
        {
            float fraction = t / time;
            gameObject.transform.parent.localScale = Vector3.Lerp(startScale, endScale, fraction);
            t += Time.deltaTime;
            yield return null;
        }
        gameObject.transform.parent.localScale = endScale;
    }
}
