using UnityEngine;

public class AttackCircleManager : MonoBehaviour
{
    public GameObject attackCirclePrefab,circulo;
    private GameObject currentAttackCircle;

    public ParticleSystem attackParticles; // Adicione particulas

   /* private void Start()
    {
        circulo = this.gameObject;

    }*/

    public void ShowAttackCircle(Vector3 position)
    {
        if (currentAttackCircle != null)
        {
            Destroy(currentAttackCircle); // Remove o círculo anterior
        }

        // Cria um círculo de ataque 
        position.y = 0.1f;
        currentAttackCircle = Instantiate(attackCirclePrefab, position, Quaternion.identity);
        currentAttackCircle.transform.localScale = new Vector3(0.5f,0.5f,0.5f);

        // Ativa o efeito de partículas
        if (attackParticles != null)
        {
            ParticleSystem particles = Instantiate(attackParticles, position, Quaternion.identity);

            particles.Play();
        }
    }

    public void HideAttackCircle()
    {
        Destroy(circulo);
    }
}
