-----------------Known Bugs---------------------------------------
+Teacher UI
	- Group Star Ranking – Order is correct, but rank numbering is sometimes the same number as overall ranking
+Student Game
	- Steal Tile does not work if players aren't in order (ex. only players 2 and 4)
		-This is because the code to determine the random player to steal from assumes the players will be in order and uses child index to determine player number
	-Pass-by steal could cause issues when the player lands on rented tile like eventlog missing a message or star not being given or deducted appropriately
+Bugs that occur after a player leaves/crashes?
	- Possible reason
		- Using GameLiftManager’s player dictionary for some implementation (probably GameReport data structure), which would result in null errors after a player leaves
	- The bugs
		- Making new questions in TeacherUI can sometimes generate multiple default question buttons
		- Unable to save or post a new question (after the multiple default buttons are generated
		- Question tile not functioning for some groups 
		- Report not generated properly for teacher (sometimes can’t download)
		- Players in the same group as the person that crashed can’t end the game
		- Rent tile won't work or cause roll dice not to work
