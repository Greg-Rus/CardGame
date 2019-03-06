using Zenject;
using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class StateMachineTests : ZenjectUnitTestFixture
{
    private FSMSystem<PlayerFSMTransitions, PlayerFSMStates> _fsm;

    [SetUp]
    public void CommonInstall()
    {
        Container.Bind<PlayerTurn>().AsTransient().NonLazy();
        Container.Bind<PlayCard>().AsTransient().NonLazy();
        Container.Bind<EndedTurn>().AsTransient().NonLazy();
        Container.Bind<Stands>().AsTransient().NonLazy();
        Container.Bind<OtherPlayerTurn>().AsTransient().NonLazy();
        Container.Bind<PlayerFsm>().AsTransient().NonLazy();

        SetupTest();
    }

    public void SetupTest()
    {
        _fsm = Container.Resolve<PlayerFsm>();
    }

    [Test]
    public void ZenjectResolvingTest()
    {
        Assert.NotNull(_fsm);
    }

    [Test]
    public void FsmTestBasicCycle()
    {
        Assert.IsTrue(_fsm.CurrentState.ID == PlayerFSMStates.PlayerTurn);
        _fsm.PerformTransition(PlayerFSMTransitions.PlayCard);
        Assert.IsTrue(_fsm.CurrentState.ID == PlayerFSMStates.PlayCard);
        _fsm.PerformTransition(PlayerFSMTransitions.PlayerTurn);
        Assert.IsTrue(_fsm.CurrentState.ID == PlayerFSMStates.PlayerTurn);
        _fsm.PerformTransition(PlayerFSMTransitions.EndTurn);
        Assert.IsTrue(_fsm.CurrentState.ID == PlayerFSMStates.EndTurn);
        _fsm.PerformTransition(PlayerFSMTransitions.PassControllToOtherPlayer);
        Assert.IsTrue(_fsm.CurrentState.ID == PlayerFSMStates.OtherPlayerTurn);
        _fsm.PerformTransition(PlayerFSMTransitions.PlayerTurn);
        Assert.IsTrue(_fsm.CurrentState.ID == PlayerFSMStates.PlayerTurn);
        _fsm.PerformTransition(PlayerFSMTransitions.Stand);
        Assert.IsTrue(_fsm.CurrentState.ID == PlayerFSMStates.Stand);
        _fsm.PerformTransition(PlayerFSMTransitions.PassControllToOtherPlayer);
        Assert.IsTrue(_fsm.CurrentState.ID == PlayerFSMStates.OtherPlayerTurn);

    }
}