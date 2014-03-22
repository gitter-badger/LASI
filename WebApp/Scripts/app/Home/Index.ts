﻿/*This file will contain all the javascript and jquery functions for Index.cshtml.
Keeping the functions will help with organization and will allow us to create classes
 with collections of javascript functions and that way we can load those classes,
which will optimize page load time.
*/

module LASI.Index {

    jobIds: (function () {

        // All top level functions should start with this directive. nested functions inherit it.
        "use strict";


        //This function disables submit button 
        $(function () {
            $("input:submit").attr("disabled", "true");
            $("input:file").change(function () {
                if ($(this).val()) {
                    $("input:submit").removeAttr("disabled");
                } else {
                    $("input:submit").attr("disabled", "true");
                }
            });
        });
        $("#submitdocumentbutton").click(e => {

            $("input:file").each((index, element: HTMLInputElement) => {
                var file = element.files[0];
                $.ajax("\\Home\\Upload", {
                    processData: false, data: file,
                    type: "POST",
                    success: (d: Object, s: string, t: JQueryXHR) => {
                        console.log(t.status);

                    },
                });
                $(e.target).css("width", "0%");
            });
            //e.preventDefault();

        });


    } ())

};
