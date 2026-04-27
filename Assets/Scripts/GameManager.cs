using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] BallController ballPrefab;
    [SerializeField] Vector2 playerSpawnPos = new(-4f, 0f);
    [SerializeField] Vector2 enemySpawnPos = new(4f, 0f);

    BallController ball;
    BallController enemy;

    public void OnPlayClicked()
    {
        if (ball == null)
        {
            ball = Instantiate(ballPrefab, playerSpawnPos, Quaternion.identity);
            ball.IsPlayer = true;

            enemy = Instantiate(ballPrefab, enemySpawnPos, Quaternion.identity);
            enemy.IsPlayer = false;
            enemy.GetComponent<SpriteRenderer>().color = Color.red;
        }
        else
        {
            ball.ResetBall(playerSpawnPos);
            enemy.ResetBall(enemySpawnPos);
        }
    }
}
