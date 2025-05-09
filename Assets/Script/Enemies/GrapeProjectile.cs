using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapeProjectile : MonoBehaviour
{
    [SerializeField] private float duration = 1f;
    [SerializeField] private AnimationCurve animCurve;
    [SerializeField] private float heightY = 3f;
    [SerializeField] private GameObject grapeProjectileShadow;
    [SerializeField] private GameObject splatterPrefab;

    private void Start()
    {
        Vector3 startPos = transform.position;
        startPos.z = 15f;

        GameObject grapeShadow = Instantiate(
            grapeProjectileShadow,
            startPos + new Vector3(0, -0.3f, 0),
            Quaternion.identity
        );

        Vector3 playerPos = Player.Instance.transform.position;
        playerPos.z = 15f;

        Vector3 grapeShadowStartPosition = grapeShadow.transform.position;
        grapeShadowStartPosition.z = 15f;

        StartCoroutine(ProjectileCurveRoutine(startPos, playerPos));
        StartCoroutine(MoveGrapeShadowRoutine(grapeShadow, grapeShadowStartPosition, playerPos));
    }

    private IEnumerator ProjectileCurveRoutine(Vector3 startPosition, Vector3 endPosition)
    {
        float timePassed = 0f;

        while (timePassed < duration)
        {
            timePassed += Time.deltaTime;
            float linearT = timePassed / duration;
            float heightT = animCurve.Evaluate(linearT);
            float height = Mathf.Lerp(0f, heightY, heightT);

            Vector2 basePos2D = Vector2.Lerp(startPosition, endPosition, linearT);
            Vector3 newPos = new Vector3(basePos2D.x, basePos2D.y + height, 15f);
            transform.position = newPos;

            yield return null;
        }

        Instantiate(splatterPrefab, new Vector3(transform.position.x, transform.position.y, 15f), Quaternion.identity);
        Destroy(gameObject);
    }

    private IEnumerator MoveGrapeShadowRoutine(GameObject grapeShadow, Vector3 startPosition, Vector3 endPosition)
    {
        float timePassed = 0f;

        while (timePassed < duration)
        {
            timePassed += Time.deltaTime;
            float linearT = timePassed / duration;
            Vector2 pos2D = Vector2.Lerp(startPosition, endPosition, linearT);
            grapeShadow.transform.position = new Vector3(pos2D.x, pos2D.y, 15f);
            yield return null;
        }

        Destroy(grapeShadow);
    }
}
