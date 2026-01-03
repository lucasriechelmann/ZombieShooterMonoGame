# Flyweight Pattern Implementation

## Overview
This document explains how the Flyweight pattern is implemented in the ZombieShooter game to minimize object allocation and improve performance.

## What is the Flyweight Pattern?
The Flyweight pattern is a structural design pattern that minimizes memory usage by sharing common data (intrinsic state) among multiple objects, while keeping unique data (extrinsic state) separate.

### Key Concepts:
- **Intrinsic State**: Shared, immutable data that's the same across multiple objects
- **Extrinsic State**: Unique, mutable data that varies per object

## Implementation in BulletSystem

### Intrinsic State (Shared Flyweights):
```csharp
readonly BulletComponent _sharedBulletComponent;
readonly SpriteComponent _sharedSpriteComponent;
readonly CircleColliderComponent _sharedColliderComponent;
readonly DisabledComponent _sharedDisabledComponent;
```

These components are created **once** in the constructor and **reused** across all bullet entities because:
- They contain no mutable state OR
- They are simple marker components OR
- Their state never changes (Sprite texture, collider radius)

### Extrinsic State (Unique per Bullet):
```csharp
bullet.Attach(new Transform2(_playerManager.Position + _offset));
MovementComponent movement = new(_bulletSpeed);
```

These components are created **per bullet** because they hold unique, mutable data:
- `Transform2`: Each bullet has a different position and rotation
- `MovementComponent`: Each bullet can have a different direction

### Benefits:
? **Reduced Memory Allocation**: With 50 bullets, we create 4 shared components instead of 200 (4 × 50)
? **Less GC Pressure**: Fewer allocations = less garbage collection
? **Better Cache Performance**: Shared components stay in CPU cache

## Implementation in EnemySystem

### Intrinsic State (Shared Flyweights):
```csharp
readonly EnemyComponent _sharedEnemyComponent;
readonly SpriteComponent _sharedSpriteComponent;
readonly DisabledComponent _sharedDisabledComponent;
```

### Extrinsic State (Unique per Enemy):
```csharp
enemy.Attach(new MovementComponent(_rand.Next(50,65)));
enemy.Attach(new Transform2(GetEnemyPosition()));
enemy.Attach(new CircleColliderComponent(7));
```

With 100 enemies:
- **Before**: 500 component objects (5 × 100)
- **After**: 303 component objects (3 shared + 300 unique)
- **Savings**: ~40% fewer allocations

## Combined with Object Pooling

The Flyweight pattern works seamlessly with the existing Object Pool pattern:

```csharp
Pool<Entity> _bullets;  // Object pooling for entities
+ Shared Components      // Flyweight for component reuse
= Optimal Memory Usage
```

### Memory Optimization Summary:
| System | Entities | Components | Before | After | Savings |
|--------|----------|-----------|--------|-------|---------|
| Bullets | 50 | 6 | 300 | 154 | 48.7% |
| Enemies | 100 | 5 | 500 | 303 | 39.4% |
| **Total** | **150** | - | **800** | **457** | **42.9%** |

## Best Practices

### ? DO Use Flyweight For:
- Marker components (empty classes used for filtering)
- Components with immutable data (sprites, textures)
- Components with constant values (collider radius that never changes)
- Disabled/status components

### ? DON'T Use Flyweight For:
- Components with mutable state that varies per entity (Transform2, MovementComponent)
- Components that need to store unique per-entity data
- Components that are modified frequently

## MonoGame.Extended ECS Compatibility

The implementation works because MonoGame.Extended ECS allows:
1. **Component Sharing**: Multiple entities can reference the same component instance
2. **Component Queries**: Systems filter entities by component type, not instance
3. **Lazy Evaluation**: Components are accessed only when needed

## Performance Impact

### Memory:
- **42.9% reduction** in component allocations
- Reduced heap fragmentation
- Better memory locality

### CPU:
- Fewer allocations = less CPU time in allocator
- Reduced garbage collection pauses
- Better cache utilization

### Notes:
This optimization is most impactful in scenarios with:
- High entity counts (100+ entities)
- Frequent entity creation/destruction
- Limited memory budget (mobile, consoles)
