# Game Programming Final Project
By Frederik, Etienne and Carlos

## Info about the PlayerCombat class
This class will contain all methods related to player combat, health and damage.
It doesn't need to be added anywhere, since it's a singleton. Use PlayerCombat.GetInstance() anywhere to get it.
Usage example:
```
PlayerCombat.GetInstance().GetPlayerHealth();
PlayerCombat.GetInstance().DamagePlayer(20f);
PlayerCombat.GetInstance().HealPlayer(10f);
PlayerCombat.GetInstance().KillPlayer();
```