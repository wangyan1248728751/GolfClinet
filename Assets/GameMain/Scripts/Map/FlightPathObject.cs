using GameFramework.ObjectPool;
using UnityEngine;

namespace Golf
{
    public class FlightPathObject : ObjectBase
    {
        public FlightPathObject(object target)
            : base(target)
        {

        }

		protected override void OnSpawn()
		{
			var obj = (FlightPathItem)Target;
			if (obj != null)
				obj.gameObject.SetActive(true);
			base.OnSpawn();
		}

		protected override void OnUnspawn()
		{
			var obj = (FlightPathItem)Target;
			if (obj != null)
				obj.gameObject.SetActive(false);
			base.OnUnspawn();
		}

        protected override void Release()
        {
            FlightPathItem flightPathItem = (FlightPathItem)Target;
            if (flightPathItem == null)
            {
                return;
            }

            Object.Destroy(flightPathItem.gameObject);
        }
    }
}
