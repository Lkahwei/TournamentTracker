# TournamentTracker
<h1>Tournament Tracker which will auto arrange the tournaments for different teams entered and assign the winner</h1><br />

<h3>There are two data storing method which are MicroSoft SQL and also in the text file</h3><br/>
<h4>Text File</h4>
![image](https://user-images.githubusercontent.com/58217794/167637721-3e541db3-d19e-4ab2-857f-04c2dc95dd93.png)

<h4>MSSQL</h4>
![image](https://user-images.githubusercontent.com/58217794/167638006-806ea198-a0b9-4ae9-81c8-8bd37414207f.png)

<h2>Dashboard</h2><br/>
<h3>Tournament Dashboard which will display the list of tournament and enable the loading of ongoing tournament and creation of new tournament</h3><br/>

![image](https://user-images.githubusercontent.com/58217794/167633885-7efa630c-5d38-4714-af50-961a73ecd13a.png)

<h2>Create New Tournament</h2><br/>
<h3>This form that enable the creation of new tournament and it will be displayed in the tournament dashboard afterwards</h3><br/>

![image](https://user-images.githubusercontent.com/58217794/167634219-091cc56c-d7d9-4521-ab14-ec0e1b4c7c6c.png)

<h2>Tournament Viewer</h2>
<h3>This form enable the user to view the tournament, the numbers of total rounds for this tournament will be depending on number of teams entered.</h3>
<h4>1.If the total number of team entered is >8 and <= 16, the numbers of round will be 4, if the number of team entered is >4 and <=8, the numbers of round will be 3 and so on.</h4>
<h4>2.The numbers of round is depending on the multiplication of two from the previous value (which is 2,4,8,16).</h4>
<h4>3.If there is insufficient team entered, there will be bias to satisfy the number of teams should be entered as stated above which means this team is automatically convert to the winner and proceed to the next round (only in the first round)</h4><br/>
<h4>4.The teams that are versus against each other is highly randomized and also the bias will be assigned randomly.</h4>
<h4>5.The user can enter the score that each team obtained, the team with higher score is considered as the winner</h4>

![image](https://user-images.githubusercontent.com/58217794/167634354-d13a35f8-4a36-4ce1-926a-4dde0765b59a.png)
<h4>6. If there round has yet to be played, the user can click the checkbox (Unplayed only) to check the match that has yet to play</h4><br/>

![image](https://user-images.githubusercontent.com/58217794/167635826-54eb5549-fb65-45be-87a3-148b688b4dd8.png)
<h4>7. If the winner has yet to be determined from the previous round, the match has yet to be determined text will be displayed</h4><br/>

![image](https://user-images.githubusercontent.com/58217794/167635676-de15c17d-a530-4b9e-915c-385ef0b727ba.png)

<h2>Create Prize form</h2><br />
<h3>This form allow the user to create the new prize for the tournament<h3><br />
  
![image](https://user-images.githubusercontent.com/58217794/167635896-f448c307-79dd-46d4-8463-2bcbaba4025e.png

<h2>Create New Team Form</h2><br />
<h3>This form allow the user to create new team and also create the new member<h3><br />
<h4>1.The user can then select the member from the dropdown or remove them if they are selected</h4><br />
<h4>2.After the user has created the new team, it will be directly displayed in the team dropdown at the create tournament page</h4><br />
  
![image](https://user-images.githubusercontent.com/58217794/167636012-3f13b25b-5a31-47f3-be35-2bf2670b637a.png)










