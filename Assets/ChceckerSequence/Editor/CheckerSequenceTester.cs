using System;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;

namespace Tim.Utility.CheckerSequence
{
	public class CheckerSequenceTester : MonoBehaviour
	{
		[Test]
		public void CheckerSequenceChecker()
		{
			IChecker ic01 = Substitute.For<IChecker>();
			ic01.When(x => x.Check(Arg.Any<Action<bool>>()))
				.Do(x => ((Action<bool>)x[0])(true));
			ic01.When(x => x.Play(Arg.Any<Action>()))
				.Do(x => ((Action)x[0]).Invoke());

			IChecker ic02 = Substitute.For<IChecker>();
			ic02.When(x => x.Check(Arg.Any<Action<bool>>()))
				.Do(x => ((Action<bool>)x[0])(false));
			ic02.When(x => x.Play(Arg.Any<Action>()))
				.Do(x => ((Action)x[0]).Invoke());

			CheckerSequence s = new CheckerSequence();
			s.Add(ic01);
			s.Add(ic02);

			s.OnPlayAllDone += () => Assert.AreEqual(1, s.PlayedCount, "PlayedCount Is {0}", s.PlayedCount);

			s.Start();

			ic01.Received().Check((Arg.Any<Action<bool>>()));
			ic02.Received().Check((Arg.Any<Action<bool>>()));

			ic01.Received().Play(Arg.Any<Action>());
			ic02.DidNotReceive().Play(Arg.Any<Action>());
		}
	}
}