using UnityEngine;

public static class ComponentExtensions
{
  public static bool HasComponent<TComponent>(this Component component) where TComponent : Component => 
    component.GetComponent<TComponent>();
}