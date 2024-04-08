- Recomano mirar l'escena "Test_01" que es veu més.
- A l'objecte terreny hi ha d'haver el component ACOManeger.

1r: 	Fer Bake grid, Recomano que a DistanceEachGridNode NO sigui inferior a 4, que sinò et pot tardar minuts. Jo de normal poso 5 o 4 (en menys de 5 segons està generat)
2n: 	Un cop creat el grid, pots donar al botó de create pathway i es pintara amb un LineRenderer el resultat 
3r: 	Dona el botó "Draw Path Ant Selected" per veure el path de la formiga -> DrawAntDebug (de moment només es generen 3 formiges)
4t:		La formiga numero 3 és la que donaria el resultat final.
4t: 	Pots provar de posar els objectes Village on vulguis de l'escena.


PD: El shader del terreny per modificar els colors s'ha de fer des del material que està: Assets/Materials/Shaders/Colors/ACO_TerrainNormalShader