using UnityEngine;

namespace MafiaTopDown.Gameplay.Runtime.Environment
{
    public sealed class RoofFadeGroup : MonoBehaviour
    {
        [SerializeField] private Renderer[] renderersToFade = new Renderer[0];

        public void SetHidden(bool hidden)
        {
            foreach (var roofRenderer in renderersToFade)
            {
                if (roofRenderer == null)
                {
                    continue;
                }

                roofRenderer.enabled = !hidden;
            }
        }
    }
}
