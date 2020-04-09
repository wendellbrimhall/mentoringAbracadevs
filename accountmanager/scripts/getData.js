﻿var getUsers;
var getEvents;
var getCurrentUser;

function getAllUsers() {
    

    var webMethod = "AccountServices.asmx/GetAllAccountInfo";
    var parameters = "{}";

    //jQuery ajax method
    $.ajax({
        type: "POST",
        url: webMethod,
        data: parameters,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (array) {
            console.log("function started");
            console.log(array);

            var infoArray = array.d;
            getUsers = infoArray;
            return infoArray;
                     
        },
            error: function (e) {
                alert("boo...");
            }       
        
    });
    
 
}

function getAllEvents() {

    var webMethod = "AccountServices.asmx/ViewEvents";
    console.log('Function started');
    var parameters = "{}";

    $.ajax({
        type: "POST",
        url: webMethod,
        data: parameters,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (array) {

            console.log(array);
            var data = array.d;

            getEvents = data;

        }

    });

}


function getCurrentUserData() {
    var webMethod = "AccountServices.asmx/ViewAccountInfo";
    var parameters = "{}";

    $.ajax({
        type: "POST",
        url: webMethod,
        data: parameters,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (array) {
            console.log('Function started');
            console.log(array);

            var infoArray = array.d;

            getCurrentUser = infoArray;
        }
    });
}