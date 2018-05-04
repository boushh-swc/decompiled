using System;
using System.Collections.Generic;

namespace Net.RichardLord.Ash.Core
{
	public class Entity
	{
		private readonly Dictionary<Type, ComponentBase> _components;

		public uint ID;

		internal event Action<Entity, Type> ComponentAdded;

		internal event Action<Entity, Type> ComponentRemoved;

		internal Entity Previous
		{
			get;
			set;
		}

		internal Entity Next
		{
			get;
			set;
		}

		internal Dictionary<Type, ComponentBase> Components
		{
			get
			{
				return this._components;
			}
		}

		public Entity()
		{
			this._components = new Dictionary<Type, ComponentBase>();
		}

		public Entity Add(ComponentBase component)
		{
			this.AddComponentAndDispatchAddEvent(component, component.GetType());
			return this;
		}

		public Entity Add(ComponentBase component, Type componentClass)
		{
			if (!componentClass.IsInstanceOfType(component))
			{
				throw new InvalidOperationException("Component is not an instance of " + componentClass + " or its parent types.");
			}
			this.AddComponentAndDispatchAddEvent(component, componentClass);
			return this;
		}

		public Entity Add<T>(ComponentBase component) where T : ComponentBase
		{
			return this.Add(component, typeof(T));
		}

		public Entity AddOrReplace<T>(ComponentBase component) where T : ComponentBase
		{
			if (this.Has<T>())
			{
				this.Remove<T>();
			}
			return this.Add<T>(component);
		}

		protected virtual Entity AddComponentAndDispatchAddEvent(ComponentBase component, Type componentClass)
		{
			if (component == null)
			{
				throw new NullReferenceException("Component cannot be null.");
			}
			while (componentClass != null)
			{
				if (componentClass == typeof(ComponentBase))
				{
					break;
				}
				if (this._components.ContainsKey(componentClass))
				{
					this._components.Remove(componentClass);
				}
				this._components[componentClass] = component;
				component.Entity = this;
				if (this.ComponentAdded != null)
				{
					this.ComponentAdded(this, componentClass);
				}
				componentClass = componentClass.BaseType;
			}
			return this;
		}

		public object Remove<T>()
		{
			return this.Remove(typeof(T));
		}

		public virtual object Remove(Type componentClass)
		{
			if (this._components.ContainsKey(componentClass))
			{
				ComponentBase componentBase = this._components[componentClass];
				componentBase.OnRemove();
				this._components.Remove(componentClass);
				componentBase.Entity = null;
				if (this.ComponentRemoved != null)
				{
					this.ComponentRemoved(this, componentClass);
				}
				return componentBase;
			}
			return null;
		}

		public void RemoveAll()
		{
			Dictionary<Type, ComponentBase>.KeyCollection keys = this._components.Keys;
			int count = keys.Count;
			Type[] array = new Type[count];
			int i = 0;
			foreach (Type current in keys)
			{
				array[i] = current;
				i++;
			}
			for (i = 0; i < count; i++)
			{
				this.Remove(array[i]);
			}
		}

		public object Get(Type componentClass)
		{
			return (!this._components.ContainsKey(componentClass)) ? null : this._components[componentClass];
		}

		public T Get<T>() where T : ComponentBase
		{
			Type typeFromHandle = typeof(T);
			return (!this._components.ContainsKey(typeFromHandle)) ? ((T)((object)null)) : ((T)((object)this._components[typeFromHandle]));
		}

		public T GetOrCreate<T>() where T : ComponentBase, new()
		{
			T t = this.Get<T>();
			if (t == null)
			{
				t = Activator.CreateInstance<T>();
				this.Add<T>(t);
			}
			return t;
		}

		public T DeepGet<T>() where T : ComponentBase
		{
			Type typeFromHandle = typeof(T);
			if (this._components.ContainsKey(typeFromHandle))
			{
				return (T)((object)this._components[typeFromHandle]);
			}
			foreach (object current in this._components.Values)
			{
				if (current is T)
				{
					return (T)((object)current);
				}
			}
			return (T)((object)null);
		}

		public List<ComponentBase> GetAll()
		{
			Dictionary<Type, ComponentBase>.ValueCollection values = this._components.Values;
			int count = values.Count;
			List<ComponentBase> list = new List<ComponentBase>(count);
			int num = 0;
			foreach (ComponentBase current in values)
			{
				list.Add(current);
				num++;
			}
			return list;
		}

		public bool Has(Type componentClass)
		{
			return this._components.ContainsKey(componentClass);
		}

		public bool Has<T>() where T : ComponentBase
		{
			Type typeFromHandle = typeof(T);
			return this._components.ContainsKey(typeFromHandle);
		}

		public bool DeepHas<T>() where T : ComponentBase
		{
			Type typeFromHandle = typeof(T);
			if (this._components.ContainsKey(typeFromHandle))
			{
				return true;
			}
			foreach (object current in this._components.Values)
			{
				if (current is T)
				{
					return true;
				}
			}
			return false;
		}
	}
}
