using UnityEngine;
using UnityEngine.Events;

namespace GeoJsonCityBuilder.Components
{
    [ExecuteAlways]
    public class TimeMachine : MonoBehaviour
    {
        public int year;
        public UnityEvent YearUpdated;
        private int lastYear;

        private void Start()
        {
            YearUpdated ??= new UnityEvent();
        }

        private void Update()
        {
            if (this.year == this.lastYear)
                return;
            this.lastYear = this.year;
            foreach (ExistenceController existenceController in (ExistenceController[])Resources.FindObjectsOfTypeAll<ExistenceController>())
            {
                if (existenceController.existencePeriodStart <= (long)this.year && (long)this.year <= existenceController.existencePeriodEnd)
                    ((Component)existenceController).gameObject.SetActive(true);
                else
                    ((Component)existenceController).gameObject.SetActive(false);
            }

            YearUpdated?.Invoke();
        }
    }
}
