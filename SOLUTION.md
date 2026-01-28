Please find my attempt at a watchlist (and notification system) for number plates. I had no problems setting up the project, and spent a bit over 6 hrs on it. At the 4 hr point the only thing I had left to setup were tests, and having angular consume the notification events via signalr.

Even though this is a demo repo & technical test, I wanted to keep it realistic to how I work, so even though the repo was forked I still created a new feature branch and kept my commits similar to how I would commit in a business. I wanted to avoid one giant commit onto main.

Which challenge I chose and why:
	I opted for the plate watchlist (using the Angular SPA) as I thought it was the most interesting out of the three. I enjoyed the variety, and that it meant I was able to work in the backend to create new SQL tables, new controllers, hubs, etc, and also make changes in the frontend using angular/typescript. I also got to play around with rabbitmq & mass transit.
	
Approach and key decisions:
	- I created new ef migrations for the database tables, and new entities for PlatesWatchlist and Notification. These have their relevant controllers/ repositories/ services which matched the patterns already used. These APIs are also RESTful, using the correct convention for endpoints, and returning correct status codes with error handling.
	
	- I also added new consumers for pre-existing Reserved, Unreserved, and Sold events. I did create a new event, but unfortunately did not have time to fully finish implementing the Price Alert feature, so it's currently unused. These consumers push real-time updates, and avoids the use of polling.
	
	- I added SignalR to handle real-time updates to the UI. As a note, I had read about SignalR before as the "go-to" approach for handling events like this in Angular, although this is my first time actually using it. As a result, I think some of the code is not great, and I spent a lot of time debugging the events because although I could see the service connecting, I couldn't actually get the notifications to appear (this was because I hadn't actually got it to join the group, which was super silly of me...)
	
	- On the frontend, there is a new Watchlist page with a simple table to show reg plates you are following. There are also new buttons on the plate-list page to add plates to the watchlist, and you can optionally set a price alert. The price-alert feature doesn't currently work, I just didn't quite get round to finishing it
	
	- On the frontend, there is also a new notificationservice that handles SignalR
	
What I'd do differently with more time:
	- Probably take more time to evaluate the code before jumping in. When I designed the tables in my head I hadn't properly checked what did/didn't exist, and so specced it out under the impression that there was a user/customer table. When I created the migration I then confused myself because I (rightfully) didn't create an index using PlateId/CustomerId because it didn't exist. Later on I thought I then missed this, added it, and broke the database setup. You can see me discover this in the commit history.
	
	- I didn't quite finish getting the Price Alert feature working, and instead pivoted to getting the notification system all setup. I wasn't too far off, but you were meant to be able to edit the price in the watchlist table which I didn't finish, and the consumer wasn't created for it.
    
    - The testing approach I took here was really poor, where I didn't write any tests until the very end, and even then they're very simple. I deliberately tried to write the code so that it could all be tested in isolation, and if I had more time I would've either followed TDD (Inside Out probably, as that's how I wrote the code anyway), or would have written tests after each method had been finished.
	
	- The Angular side is also unfortunately not ideal. I was beginning to feel the time pressure at this point, and as a result there is at least one bug I know off where to get the watchlist page to load you need to click the link twice. I think this is a routing bug, but I didn't really look into it as it was functional enough.
	
	- For the most part I also tried to follow the patterns already in the codebase, so the code was consistent. However, this meant that I did not write any DTOs as Plate was not using them, and equally there should be some user or customer table. The only place where this differed was in Angular, where I used the new approach (e.g. @for instead of *nGFor) as the old approach is depreceated. With more time, I would've liked to update this.
	
Any trade-offs you made:
	- Looking through the code whilst writing up this file I've realised that the implementation of the consumers isn't actually very scalable. If you wanted to add an email notification then you'd have to add it to the consumer, which isn't really ideal. I think this should have a separate notification service, and so the consumer of PriceAlertTriggeredIntegrationEvent would send the notification.