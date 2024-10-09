To start project you have to do some things.

You have .env.example file. Read it.
Then you should to create .env file and add this env variables with your data.

#LiqPay

ServerOptions__ServerUrl => Your application url. Liq pay have to know where send callback(data about payment).

For example your server url https://yourPoshtaApp.com
In .env set ServerOptions__ServerUrl=https://yourPoshtaApp.com
In LiqPay site set CALLBACK URL(NOT RESULT URL) https://yourPoshtaApp.com/api/payment/process-payment-result

/api/payment/process-payment-result is your endpoint.

#TEST

To start test go to Poshta.UnitTests or Poshta.Integration.Tests and use command: [dotnet test]

#DOCKER

You can start project with docker

Go to project and use command: [docker-compose up --build] then application will work at 5001 port.
Then go to https://localhost:5001/swagger/index.html and use swagger. Here you can simply understend about endpoints.

To change port go to docker-compose.yml.

#ABOUT PROJECT

It's poshta application that's why you have such entities:

User. Has email, phone, last/first/middle name, password, role...(Identity)

Operator (userId, postOfficeId) has rating. Work in post office and can do manipulation with shipments in this post office.
	Ñan issue shipments. Cand accept in postOffice, can send from post office.
	If shipment was redirected automatically created new shipment and operator have to send NEW SHIPMENT(Same but with another id).
	Has role operator and can be created by user with Admin role.

Shipment. Has sender, recipient, confidant(if sender or recipient want to add him), 
	start/end/current post office, shipment history, status, price...

Post office type. For ex: PostOfficeUpTo30Kg, PostOfficeUpTo15Kg... can create, remove only user with Admin role

Post office: Has post office type, Name, Max shipments count, Operators who works here.
	Can be created/deleted by user with Admin role

TO START WORK:

Your task is to go to the admin test account and give yourself an admin. then delete the admin test account

Use swagger(much better for start) or postman https://localhost:5001/api/notifications/sms-send-code Your phone in body.

Then https://localhost:5001/api/users/registration Your data and code from sms. If code expired try again(sms-send-core => registration)

I create test seed data. Some users(1 with admin role) Post office types, Post offices

Admin phone number: +1234567890.
Admin password: adminPassword.

Go to https://localhost:5001/api/users/login and use ADMIN phone and pass. (Not your)

Now you login Admin test account.

Go to https://localhost:5001/api/users (now you have permission to do it) YOUR ID (not test user id).
Then you should go to https://localhost:5001/api/users/YOUR_ID/add-role and set ADMIN or Admin.

Then go to https://localhost:5001/api/users/login and login with you phone and password.

Remove Test admin.

Go to https://localhost:5001/api/users/TEST_ADMIN_ID/delete

Test admin Id: 00000000-0000-0000-0000-000000000001 or you can see in https://localhost:5001/api/users/

You can remove post office, post office type, users test data and create your.

If you create Operator(role: "Operator" add automatically)