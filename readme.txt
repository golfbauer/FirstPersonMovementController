Projketname:	
		FirstPersonMovementController 

Teammitglieder:
		Henri Truetsch [206305]
		Ansgar Koopmann [205107]

Starten/Bedienung des Projektes:
Keine Besonderheiten beim Starten des Projektes. Unter Assets/Scenes kann das Testlevel oder das Parkourlevel geöffnet werden. 

Tastenbelegung:
	Walk: WASD
	Sprint: Shift + WASD
	Jump: Space (mehrfach für multi jump)
	Crouch: C (Toggle)
	Slide: C während Sprint
	Dash: Shift in Luft
	Wallrun: Leertaste halten an einer Wand(funktioniert nich an allen Wänden)
	Walljump: Während Wallrun Leertaste loslassen
	Grapple Hook: F (funktioniert nich an allen Wänden)
	Jetpack: E halten

Leistungen, Herausforderungen und Erfahrungen:
Für uns beide was unser erstes Projekt mit Unity und C# was dementsprechend anfangs zu einer steilen Lernkurve geführt hat. 

Zu Beginn war eine große Herausforderung die collision für unseren Spieler per Kinamatischem Rigidbody umzusetzen. 
Es gab sehr viele Tutorials für Unitys eingebauten CharacterController aber für unseren Ansatz fiel es uns schwer qualitative Informationen zu finden. 
Nach viel herumsuchen und probieren könnten wir glücklicherweise das Projekt OpenKCC (siehe Attributions) finden welches uns hier sehr weitergeholfen hat. 

Die größten Herausforderungen bezüglich Features hatten wir bei Crouching, Stair Movement und Plattform Movement. 

Beim crouching wird die Größe und Position des Colliders verändert was oft dazu führen konnte, dass der Spieler im Boden oder, beim Beenden des crouchen, in der Decke stecken blieb.
Diese Probleme waren besonders herausfordernd, wenn noch andere Faktoren wie Schrägen oder Treppen dazukamen.

Stair Movement benötig sehr viel Präzision, um zu verhindern in dem Objekt steckenzubleiben, auf das man sich bewegen will und gleichzeitig nicht über das Objekt platziert wird was
sich unnatürlich anfühlt. Außerdem müssen viele Sonderfälle beachtet werden z.B. an Kanten von Objekten, bei Stufen auf Schrägen oder bei Laufen auf Stufen in einem steilen Winkel. 
 
Bei dem Plattform Movement gab es einige Herausforderungen, um das Momentum korrekt zu berechnen, wenn die Plattform verlassen wird und um nicht an Objekten zu „haften“,
welche man nur seitlich berührt aber auf denen man nicht steht. 

Weitere Herausforderungen waren unser Movementsystem Momentum basiert zu machen und unseren Controller zu restrukturieren.
Für diese Probleme mussten wir die Architektur unseres Controllers umdenken und große Teile unseres Codes umschreiben, was einiges an Zeit gekostet hat. 


Attributions:
Für unsere collision detection haben wir code aus dem Projekt OpenKCC verwendet: https://github.com/nicholas-maltbie/OpenKCC
Der verwendete Code befindet sich in der Datei Scripts/MovementController/KinematicCharacterController.cs 

Video:
Alle Features: https://drive.google.com/file/d/1gQMbmlKHhk4S09Tzbh--O6dx-QPbpwjn/view
Parkour Demo: https://drive.google.com/file/d/1UccO5vV5F4x89grJOdeYrsXgSbRwho7C/view?usp=share_link

Weitere Links:
Google Drive: https://drive.google.com/drive/folders/17-H06iXHG8AFzqKjunwbQ1Nj-bY2_Fgp
Trello Board: https://trello.com/b/SwB0X7WD/firstpersonmovementcontroller
Github Repo: https://github.com/golfbauer/FirstPersonMovementController