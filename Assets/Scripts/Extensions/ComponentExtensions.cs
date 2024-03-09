using UnityEngine;

namespace Extensions
{
  public static class ComponentExtensions
  {
    public static bool HasComponent<TComponent>(this Component component) where TComponent : Component =>
      component.TryGetComponent(out TComponent _);
  }
}