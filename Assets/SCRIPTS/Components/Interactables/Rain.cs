using ANT.Components.Audio;

using UnityEngine;

namespace ANT.Components.Interactables {
    public class Rain : MonoBehaviour {
        [SerializeField] private GameObject WaterDropPrefab;
        [SerializeField] private float RainingRate;
        [Range(10, 20)]
        [SerializeField] private float RainingWidth;

        private float _rainingDelta;

        private bool _raining;

        private void Start() {
            _rainingDelta = RainingRate;
        }

        private void Update() {
            if (!_raining) return;

            if(_rainingDelta <= 0) Raining();

            _rainingDelta -= Time.deltaTime;
        }

        private void Raining() {
            float spawnX = Random.Range(-RainingWidth, RainingWidth);
            spawnX = Mathf.Round(spawnX * 100f) / 100f;
            Vector3 spawnPosition = new Vector3(transform.position.x + spawnX, transform.position.y);

            Instantiate(WaterDropPrefab, spawnPosition, Quaternion.identity);

            _rainingDelta = RainingRate;
        }

        private void OnTriggerEnter2D(Collider2D collision) {
            if(!collision.CompareTag("Game/PlayableAnt")) return;

            _raining = true;
            SoundManager.Instance.Play("Rain");
        }
    }
}
