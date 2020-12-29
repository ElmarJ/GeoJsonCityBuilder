using UnityEngine;

namespace GeoJsonCityBuilder
{
  [ExecuteAlways]
  public class TimeMachine : MonoBehaviour
  {
    public int year;
    private int lastYear;

    private void Update()
    {
      if (this.year == this.lastYear)
        return;
      this.lastYear = this.year;
      foreach (ExistenceController existenceController in (ExistenceController[]) Resources.FindObjectsOfTypeAll<ExistenceController>())
      {
        if (existenceController.existencePeriodStart <= (long) this.year && (long) this.year <= existenceController.existencePeriodEnd)
          ((Component) existenceController).gameObject.SetActive(true);
        else
          ((Component) existenceController).gameObject.SetActive(false);
      }
    }
  }
}
