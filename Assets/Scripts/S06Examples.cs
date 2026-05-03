using System.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class S06Examples : MonoBehaviour
{
    Coroutine mycoroutine;

    public AnimationCurve curve;

    void Start()
    {
        Time.timeScale = 1f;
        /*
        //ExampleMethod();

        // StartCoroutine(nameof(SimpleCoroutine));

        mycoroutine = StartCoroutine(Countdown(10));

       
        StopAllCoroutines();

        StartCoroutine(CountdownRealtime(10));


        Invoke(nameof(ExampleMethod), 2f);
        InvokeRepeating(nameof(ExampleMethod), 0, 5);*/

        //StopCoroutine(mycoroutine);


        StartCoroutine(MoveNattack(new Vector3(0, 0, 10)));
    }

    public IEnumerator MoveNattack(Vector3 finalPos)
    {
        Transform currentTransfor = gameObject.transform;
        float maxTime = 5;
        float currentTime = 0;

        while (currentTime < maxTime)
        {


            float curveValue = curve.Evaluate(currentTime/maxTime);


            gameObject.transform.position = Vector3.Lerp(currentTransfor.position, finalPos, curveValue);//->resolver :D

            currentTime += Time.deltaTime;

            yield return null;
        }
        gameObject.transform.position = finalPos;
        StartCoroutine(nameof(RotateInfinity));

        yield return null;
    }

    public IEnumerator RotateInfinity()
    {

        while (true)
        {
            transform.eulerAngles += Vector3.up * 30 * Time.deltaTime;
            yield return null;
        }
       // yield break;
    }

    public IEnumerator Attack()
    {
        Transform currentTransfor = gameObject.transform;
        quaternion finalRotation = quaternion.Euler(currentTransfor.eulerAngles + new Vector3(0,90,0));

        float maxTime = 5;
        float currentTime = 0;
        Debug.Log("Attack");

        while (currentTime < maxTime)
        {
            gameObject.transform.rotation = Quaternion.Lerp(currentTransfor.rotation, finalRotation, currentTime / maxTime);

            currentTime += Time.deltaTime;

            Debug.Log("Attack");

            yield return null;
        }
        gameObject.transform.rotation = finalRotation;

        yield return null;
    }



    void Update()
    {
        
    }
    public void ExampleMethod()
    {
        Debug.Log("A");
        Debug.Log("B");
        Debug.Log("C");
    }

    public IEnumerator SimpleCoroutine()
    {



        Debug.Log("A");
        yield return new WaitForSeconds(1f);
        Debug.Log("B");
        yield return new WaitForSeconds(1f);
        Debug.Log("C");
    }

    public IEnumerator Countdown(int value)
    {
        int count = value;

        while (count > 0)
        {

            if (value >= 1000)
            {
                //->reiniciar stats
                yield break;
            }

            Debug.Log("c"+count);
            count--;
            yield return new WaitForSeconds(1f);
        }


        yield break;
    }
    public IEnumerator CountdownRealtime(int value)
    {
        int count = value;

        while (count > 0)
        {

            if (value >= 1000)
                yield break;

            Debug.Log("r"+count);
            count--;
            yield return new WaitForSecondsRealtime(1f);
        }


        yield break;
    }


    public bool IsGround = false;
    public IEnumerator playUntilSmt()
    {
        int count = 20;

        while (count > 0)
        {

            yield return new WaitUntil( () =>  IsGround);//-            =>



            Debug.Log(count);
            count--;
            yield return new WaitForSeconds(1f);
        }


        yield break;
    }
}
