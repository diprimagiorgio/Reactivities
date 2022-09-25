Action()
{

	web_set_sockets_option("SSL_VERSION", "AUTO");

	web_set_max_html_param_len("454");

	web_url("tutorial-reactivities.herokuapp.com", 
		"URL=https://tutorial-reactivities.herokuapp.com", 
		"TargetFrame=", 
		"Resource=0", 
		"RecContentType=text/html", 
		"Referer=", 
		"Snapshot=t1.inf", 
		"Mode=HTML", 
		EXTRARES, 
		"Url=/static/js/2.ad2019d9.chunk.js", "Referer=", ENDITEM, 
		"Url=/assets/logo.png", "Referer=", ENDITEM, 
		LAST);

/*Correlation comment - Do not change!  Original value='eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJuYW1lIjoidGVzdGVyMjYwOCIsIm5hbWVpZCI6IjMxMWY4ZTU5LTk2OTAtNDM3Yy1hZWNmLTVkN2JjOGZmOTc5ZCIsImVtYWlsIjoidGVzdGVyMjYwOEBnbWFpbC5jb20iLCJuYmYiOjE2NjE1MDIyNTYsImV4cCI6MTY2MjEwNzA1NiwiaWF0IjoxNjYxNTAyMjU2fQ.qkg7sho6QLPZGLXLj12JNR5Q0cklyR0ZoY6BAjxef3IFBCi24x-xdOrXHzLI2shsjjwYyX1X5XMpgSuzoXss_Q' Name ='access_token' Type ='RecordReplay'*/
	web_reg_save_param_json(
		"ParamName=access_token",
		"QueryString=$.token",
		SEARCH_FILTERS,
		"Scope=Body",
		LAST);

	web_custom_request("login", 
		"URL=https://tutorial-reactivities.herokuapp.com/api/account/login", 
		"Method=POST", 
		"TargetFrame=", 
		"Resource=0", 
		"RecContentType=application/json", 
		"Referer=", 
		"Snapshot=t2.inf", 
		"Mode=HTML", 
		"EncType=application/json", 
		"Body={\"email\":\"tester2608@gmail.com\",\"password\":\"Test1!\",\"error\":null}", 
		EXTRARES, 
		"Url=/assets/user.png", "Referer=", ENDITEM, 
		LAST);
		
		//web_add_header("Content-Type", "application/json");
		//web_add_header("Authorization", "(Bearer {access_token})" );
		web_set_user("tutorial-reactivities\\tester2608@gmail.com", "Test1!", "tutorial-reactivities.herokuapp.com:443");
	web_url("activities", 
		"URL=http://tutorial-reactivities.herokuapp.com/api/activities?pageNumber=1&pageSize=2&all=true", 
		"TargetFrame=", 
		"Resource=0", 
		"RecContentType=application/json", 
		"Referer=", 
		"Snapshot=t3.inf", 
		"Mode=HTML", 
		EXTRARES, 
		"Url=../static/media/icons.38c6d8ba.woff2", "Referer=", ENDITEM, 
		LAST);

	web_url("activities_2", 
		"URL=https://tutorial-reactivities.herokuapp.com/api/activities?pageNumber=2&pageSize=2&all=true", 
		"TargetFrame=", 
		"Resource=0", 
		"RecContentType=application/json", 
		"Referer=", 
		"Snapshot=t4.inf", 
		"Mode=HTML", 
		EXTRARES, 
		"Url=../assets/categoryImages/food.jpg", "Referer=", ENDITEM, 
		LAST);

	web_custom_request("negotiate",
		"URL=http://tutorial-reactivities.herokuapp.com/chat/negotiate?activityId=6af44a24-89b5-494b-947b-f12cd3a07991&negotiateVersion=1",
		"Method=POST",
		"TargetFrame=",
		"Resource=0",
		"RecContentType=application/json",
		"Referer=",
		"Snapshot=t5.inf",
		"Mode=HTML",
		"EncType=text/plain;charset=UTF-8",
		LAST);

	lr_think_time(4);

	web_websocket_connect("ID=0",
		"URI=ws://tutorial-reactivities.herokuapp.com/chat?activityId=6af44a24-89b5-494b-947b-f12cd3a07991&id=ilcyL2QhET67SCKkZwZ0NQ&access_token={access_token}",
		"Origin=http://tutorial-reactivities.herokuapp.com",
		"OnOpenCB=OnOpenCB0",
		"OnMessageCB=OnMessageCB0",
		"OnErrorCB=OnErrorCB0",
		"OnCloseCB=OnCloseCB0",
		LAST);

	web_custom_request("attend",
		"URL=http://tutorial-reactivities.herokuapp.com/api/activities/6af44a24-89b5-494b-947b-f12cd3a07991/attend",
		"Method=POST",
		"TargetFrame=",
		"Resource=0",
		"RecContentType=application/json",
		"Referer=",
		"Snapshot=t7.inf",
		"Mode=HTML",
		"EncType=application/json",
		"Body={}",
		LAST);

	web_websocket_send("ID=0", 
		"Buffer={\"protocol\":\"json\",\"version\":1}\x1E", 
		"IsBinary=0", 
		LAST);

	/*Connection ID 0 received buffer WebSocketReceive0*/

	/*Connection ID 0 received buffer WebSocketReceive1*/

	web_custom_request("attend_2",
		"URL=http://tutorial-reactivities.herokuapp.com/api/activities/6af44a24-89b5-494b-947b-f12cd3a07991/attend",
		"Method=POST",
		"TargetFrame=",
		"Resource=0",
		"RecContentType=application/json",
		"Referer=",
		"Snapshot=t8.inf",
		"Mode=HTML",
		"EncType=application/json",
		"Body={}",
		LAST);

	web_websocket_close("ID=0", 
		"Code=1000", 
		LAST);

	return 0;
}