#Readme Speed Camera Mesta 210d


Package containing 2 radar prefabs (new and old)

The radar prefab contains in:
- 3D model for lod1
- 3D model for lod2
- one or more triggers with the Radar.cs scripts
- a light to simulate the flash

the Radar script calculates the speed of the vehicle when it enters the collider trigger. If the vehicle exceeds the maximum speed then the radar emits a flash.
SpeedMax: flash firing speed
KinematicVelocity: bool if we recover the velocity of the vehicle otherwise it is calculated via the position
light: Radar light gameobject

vehicles must have a Collider component with isTrigger true, and a kinematic rigidBody.

//================================================================================================================================

Package contenant 2 prefabs ( neuf et vieux) de radar

Le prefab radar se décomposent en:
- modele 3D pour le lod1
- modele 3D pour le lod2
- un ou plusieurs triggers avec le scripts Radar.cs
- une light pour simuler le flash

le script Radar calcule la vitesse du vehicule lorsqu'il rentre dans le collider trigger.Si le vehicule dépasse la vitesse maximale alors le radar émet un flash.
SpeedMax : vitesse de declenchement du flash
KinematicVelocity: bool si on recupére la velocity du vehicule sinon elle est calculée via la position
light: Gameobject de la light du radar

les vehicules doivent posséder un component Collider avec isTrigger a true, et un rigidBody en kinematic.