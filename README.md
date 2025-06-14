# Reddit Image Scheduler

Reddit Image Scheduler is currently **INCOMPLETE**, and can't post anything. The application has been created as an exercise, with hopes that perhaps one day Reddit will enable image posting through their API (which currently is not possible).

## Description

Reddit Image Scheduler (RIS) is a cross-platform C# application created using the [Eto framework](https://github.com/picoe/Eto), [SQLite-net](https://github.com/praeclarum/sqlite-net), and the [Reddit.NET library](https://github.com/sirkris/Reddit.NET).  
RIS can be compiled to any of the .NET Standard 2.0 targets that [Eto supports](https://github.com/picoe/Eto?tab=readme-ov-file#currently-supported-targets).

In its current state, Reddit Image Scheduler implements:

1.  **Authentication.** You can register RIS as an app for Reddit and then grant it access to post on your behalf—although actual posting is not implemented.
    
2.  **Post scheduling.** You can add a post to the schedule and select a date for posting. The post can have a title, a URL source (intended to be included in the body of the post), and the image itself. You **can't** select a target subreddit, post flair, or NSFW toggle.
    
3.  **Timetable preview.** On a separate view, you can watch a real-time countdown tick by until each post is submitted. Although again, actual posting is not implemented, and nothing happens after the timer ends.
    
4.  **Tray icon.** RIS works in the background, hidden away in your tray.
    
5.  **Drag-and-drop editing.** Images can be dropped onto the editor UI.
    

## Usage

To run the application, simply clone the repository and open the project in your favorite IDE. All required references should be downloaded as NuGet packages.

Once the project is up and running, you'll be presented with the login form. Press **Register** to open a webpage where you can register your Reddit app. Or [click here](https://www.reddit.com/prefs/apps) in case the button doesn't do anything (for some reason). Select **installed app** and copy-paste the **redirect URI** from the RIS login UI. You can't change it later, so make sure to paste the right thing.  
![Login form](imgs/login_form.png)  
![Reddit app](imgs/reddit_app.png)  
On successful registration, you'll be assigned an **App ID**, which you can copy and paste into the login form. The **App secret** is not required and can be left empty.  

Press **Connect** to try and grant access to the application - a new webpage should open where you can press **Allow**. If everything goes smoothly, you should see the login form close and the RIS icon appear in your tray. Either click it, or right-click it to open the context menu where you can select **Edit**. To permanently close the application, you have to right-click the tray icon and select **Quit**.

## TODO

1.  Actual image posting once Reddit API supports it.
2.  Target subreddit selection.
3.  Flair selection.
4.  NSFW toggle.
