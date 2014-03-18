﻿module LASI.Progress {
    export class Status {
        formattedPercent: string;
        constructor(public message: string, public percent: number) {
            this.formattedPercent = percent.toString() + "%";
        }
        static fromJson(jsonString: string): Status {
            var js = JSON.parse(jsonString);
            return new Status(js.message, js.percent);
        }
    }
}


$(() => {
    // Import class Status
    var Status = LASI.Progress.Status;


    // Sets listening for progress automatically.
    // This needs to be refactored into more re-usable code.
    var jobId = (function () {
        // Gets all ungoing jobs from the server and generates a new
        // Id number by using a bitwise xor
        var id = $.makeArray($.getJSON("./GetJobStatus"))
            .map((x: any, i: number) => x.id)
            .reduce((sofar: number, x: number) => sofar ^ x, 0);
        return (function () {
            setInterval(event => {
                $.getJSON("./GetJobStatus?jobId=" + jobId,
                    function (data, status, jqXhr) {
                        var st = Status.fromJson(data);
                        var $progress = $(".progress-bar");
                        $progress.css("width", st.percent);
                        $progress.text(st.message);
                    });
            }, 1000);
            return id += 1;
        })();
    })();
});
