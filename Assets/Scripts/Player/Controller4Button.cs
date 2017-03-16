using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller4Button : Controller2Button
{
	protected override void input()
	{
		// NOT REALLY SURE WHAT TO DO HERE



		
		if(Input.GetAxisRaw("Hand1") > 0 && t1 != null)
		{
			t1 = target();
			mag.force(t1.gameObject, true);
		}

		if(Input.GetAxisRaw("Hand2") > 0 && t2 != null)
		{
			t2 = target();
			mag.force(t2.gameObject, true);
		}	
	}
}
