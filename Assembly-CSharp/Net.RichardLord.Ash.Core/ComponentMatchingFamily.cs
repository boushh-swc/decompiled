using Net.RichardLord.Ash.Internal;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Net.RichardLord.Ash.Core
{
	public class ComponentMatchingFamily<TNode> : IFamily where TNode : Node<TNode>, new()
	{
		private static readonly string[] reservedProperties = new string[]
		{
			"Entity",
			"Entities",
			"Previous",
			"Next"
		};

		private Dictionary<Entity, TNode> entities;

		private Dictionary<Type, string> components;

		private NodePool<TNode> nodePool;

		private Game game;

		public void Setup(Game game)
		{
			this.game = game;
			this.Init();
		}

		private void Init()
		{
			this.nodePool = new NodePool<TNode>();
			this.entities = new Dictionary<Entity, TNode>(400);
			this.components = new Dictionary<Type, string>(10);
			PropertyInfo[] properties = typeof(TNode).GetProperties(BindingFlags.Instance | BindingFlags.Public);
			int i = 0;
			int num = properties.Length;
			while (i < num)
			{
				PropertyInfo propertyInfo = properties[i];
				if (Array.IndexOf<string>(ComponentMatchingFamily<TNode>.reservedProperties, propertyInfo.Name) == -1)
				{
					if (!this.components.ContainsValue(propertyInfo.Name))
					{
						this.components.Add(propertyInfo.PropertyType, propertyInfo.Name);
					}
				}
				i++;
			}
		}

		public void NewEntity(Entity entity)
		{
			this.AddIfMatch(entity);
		}

		public void ComponentAddedToEntity(Entity entity, Type componentClass)
		{
			this.AddIfMatch(entity);
		}

		public void ComponentRemovedFromEntity(Entity entity, Type componentClass)
		{
			if (this.components.ContainsKey(componentClass))
			{
				this.RemoveIfMatch(entity);
			}
		}

		public void RemoveEntity(Entity entity)
		{
			this.RemoveIfMatch(entity);
		}

		private void AddIfMatch(Entity entity)
		{
			if (!this.entities.ContainsKey(entity))
			{
				Dictionary<Type, string>.KeyCollection keys = this.components.Keys;
				bool flag = false;
				foreach (Type current in keys)
				{
					if (!entity.Has(current))
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					return;
				}
				TNode tNode = this.nodePool.Get();
				tNode.Entity = entity;
				foreach (Type current2 in this.components.Keys)
				{
					tNode.SetProperty(this.components[current2], entity.Get(current2));
				}
				this.entities.Add(entity, tNode);
				NodeListSingleton<TNode>.NodeList.Add(tNode);
			}
		}

		private void RemoveIfMatch(Entity entity)
		{
			TNode node;
			if (this.entities.TryGetValue(entity, out node))
			{
				this.entities.Remove(entity);
				NodeListSingleton<TNode>.NodeList.Remove(node);
				if (this.game.Updating)
				{
					this.nodePool.Cache(node);
					this.game.UpdateSimComplete += new Action(this.ReleaseNodePoolCache);
					this.game.UpdateViewComplete += new Action(this.ReleaseNodePoolCache);
				}
				else
				{
					this.nodePool.Dispose(node);
				}
			}
		}

		private void ReleaseNodePoolCache()
		{
			this.game.UpdateSimComplete -= new Action(this.ReleaseNodePoolCache);
			this.game.UpdateViewComplete -= new Action(this.ReleaseNodePoolCache);
			this.nodePool.ReleaseCache();
		}

		public void CleanUp()
		{
			FamilySingleton<TNode>.Family = null;
		}
	}
}
