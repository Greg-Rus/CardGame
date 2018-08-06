using Zenject;
using UnityEngine;
using System.Collections;

public class BasicInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<int>().FromInstance(1);
        Container.Bind<Greeter>().AsSingle().NonLazy();
    }
}

public class Greeter
{
    public Greeter(int message)
    {
        Debug.Log(message);
    }
}