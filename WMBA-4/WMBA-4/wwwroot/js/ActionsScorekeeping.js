$(document).ready(function () {
    /* Close popup  inplay */
    $('#closeButtonInplay').click(function () {
        // Solo cierra el popup
        $('#inplayModal').modal('hide');
    });

   /* Close button Inplay from  x */
        $('#closeButtonInplayx').click(function () {
            // Solo cierra el popup
            $('#inplayModal').modal('hide');
        });

 /* For the Modal Inplay */
        $('#openPopupButton').click(function () {
        var inplayId = $('#inplayId').val();
        $.ajax({
            url: '/ScorePlayer/LoadInplayPartial',
        data: {
            inplayId: inplayId
            },
        success: function (response) {

            $('#inplayModalBody').html(response);

        $('#inplayModal').modal('show');
            }
        });
        });

/*  For saving the changes in the modal*/
        $(document).ready(function () {
            $('#saveChangesButton').click(function () {
                var data = {
                    InplayID: Number($('#inplayId').val()),
                    PlayerInBase1Base: Number($('input[name="PlayerInBase1Base"]:checked').val()),
                    PlayerInBase2Base: Number($('input[name="PlayerInBase2Base"]:checked').val()),
                    PlayerInBase3Base: Number($('input[name="PlayerInBase3Base"]:checked').val()),
                    PlayerBattingBase: Number($('input[name="PlayerBattingBase"]:checked').val()),
                    PlayerInBase1Id: Number($('#playerInBase1Id').val()),
                    PlayerInBase2Id: Number($('#playerInBase2Id').val()),
                    PlayerInBase3Id: Number($('#playerInBase3Id').val()),
                    PlayerBattingId: Number($('#playerBattingId').val()),
                    IsHit: $('input[name="isHit"]').is(':checked'),
                    IsRunPlayer1: $('input[name="isRunPlayer1"]').is(':checked'),
                    IsRunPlayer2: $('input[name="isRunPlayer2"]').is(':checked'),
                    IsRunPlayer3: $('input[name="isRunPlayer3"]').is(':checked'),
                    IsOutPlayer1: $('input[name="isOutPlayer1"]').is(':checked'),
                    IsOutPlayer2: $('input[name="isOutPlayer2"]').is(':checked'),
                    IsOutPlayer3: $('input[name="isOutPlayer3"]').is(':checked'),
                    IsRBI: Number($('input[name="isRBI"]').val()),
                    IsHomerun: $('input[name="isHomerun"]').is(':checked'),

                };

                $.ajax({
                    url: '/ScorePlayer/SaveInplayChanges',
                    type: 'POST',
                    contentType: 'application/json',
                    data: JSON.stringify(data),
                    success: function (response) {
                        console.log(response);
                        if (response.success) {
                            console.log('Los datos se guardaron correctamente..');
                            $('#inplayId').val(response.inplay.id);
                            $('#strikesCount').text(response.inplay.strikes);
                            $('#outs').text(response.inplay.outs);
                            $('#fouls').text(response.inplay.fouls);
                            $('#ballsCount').text(response.inplay.balls);
                            $('#runs').text(response.inplay.runs);
                            $('#runs2').text(response.inplay.runs2);
                            $('#inning').text(response.inplay.inningNumber);
                            $('#playerAtBat').text(response.inplay.firstPlayer);

                            // Cierra el popup
                            $('#inplayModal').modal('hide');
                        } else {
                            console.log('Hubo un error al guardar los datos de Inply');
                        }
                    }
                });
            });
    });

/*  Script for Button´s action */
        $(document).ready(function () {

            // Start Button
            $('#startButton').click(function (e) {
                e.preventDefault();

                var inplayId = $('#inplayId').val();
                var url = $(this).attr('href');
                url = url.replace('http://', 'https://');

                $.ajax({
                    url: url,
                    type: 'GET',

                    success: function (data) {
                        if (data.success === false) {
                            // Show the error message in an alert
                            alert(data.message);
                        } else if (data.inplay) {
                            console.log(data.inplay);

                            $('#inplayId').val(data.inplay.id);
                            $('#strikesCount').text(data.inplay.strikes);
                            $('#outs').text(data.inplay.outs);
                            $('#fouls').text(data.inplay.fouls);
                            $('#ballsCount').text(data.inplay.balls);
                            $('#runs').text(data.inplay.runs);
                            $('#runs2').text(data.inplay.runs2);
                            $('#inning').text(data.inplay.inningNumber);
                            $('#playerAtBat').text(data.inplay.firstPlayer);
                            $('#isVisitor').val(data.inplay.isVisitor);

                            // Check if isVisitor is false
                            if (!data.inplay.isVisitor) {
                                // Hide the buttons              
                                $('#ballsButton, #strikeButton, #foulButton, #outButton, #hbpButton,#hitButton,#openPopupButton,#startButton,#inningButton,#lineupButton,#startButton').hide();
                            } else {
                                // Show the buttons
                                $('#opponentScoreButton, #opponentOutsButton,#startButton,#lineupButton').hide();
                            }


                        } else {
                            console.log('inplay is undefined');
                        }
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        console.log(textStatus, errorThrown);
                    }
                });
            });


        // Opponent Score Button
        $('#opponentScoreButton').click(function (e) {
            e.preventDefault();
        var teamId = $(this).data('teamid');
        var gameId = $(this).data('gameid');
        var inplayId = $('#inplayId').val();

        $.ajax({
            url: '/ScorePlayer/OpponentScore',
        type: 'POST',
        data: {inplayId: inplayId, teamId: teamId, gameId: gameId },
        success: function (data) {
                    if (data.Error) {
            alert(data.Error);
        return;
                    }
        if (data.success === false) {
            alert(data.message);
        return;
                    }

        $('#ballsCount').text(data.inplay.balls);
        $('#inplayId').val(data.inplay.id);
        $('#strikesCount').text(data.inplay.strikes);
        $('#outs').text(data.inplay.outs);
        $('#fouls').text(data.inplay.fouls);
        $('#runs').text(data.inplay.runs);
        $('#runs2').text(data.inplay.runs2);
        $('#inning').text(data.inplay.inningNumber);
        $('#playerAtBat').text(data.inplay.firstPlayer);


        // Actualiza las celdas de la tabla para el equipo "A"
        var inningScoresTeam1 = data.inningScoresTeam1;
        for (var key in inningScoresTeam1) {
                        if (key !== "$id") {  // Ignora la clave "$id"
                            var score = inningScoresTeam1[key];
        console.log($('#inningScoreA' + key));
        $('#inningScoreA' + key).text(score);
                        }
                    }

        // Actualiza las celdas de la tabla para el equipo "H"
        var inningScoresTeam2 = data.inningScoresTeam2;
        for (var key in inningScoresTeam2) {
                        if (key !== "$id") {  // Ignora la clave "$id"
                            var score = inningScoresTeam2[key];
        console.log($('#inningScoreH' + key));
        $('#inningScoreH' + key).text(score);
                        }
                    }

                },
        error: function (jqXHR, textStatus, errorThrown) {
            console.log(textStatus, errorThrown);
                }
            });
        });



       // Opponent Outs Button
            $('#opponentOutsButton').click(function (e) {
                e.preventDefault();
                var teamId = $(this).data('teamid');
                var gameId = $(this).data('gameid');
                var inplayId = $('#inplayId').val();

                $.ajax({
                    url: '/ScorePlayer/OpponentOuts',
                    type: 'POST',
                    data: { inplayId: inplayId, teamId: teamId, gameId: gameId },
                    success: function (data) {
                        if (data.Error) {
                            alert(data.Error);
                            return;
                        }
                        if (data.success === false) {
                            alert(data.message);
                            return;
                        }
                        console.log(data.inplay);
                        console.log(data.inplay.isVisitor); 
                        $('#ballsCount').text(data.inplay.balls);
                        $('#inplayId').val(data.inplay.id);
                        $('#strikesCount').text(data.inplay.strikes);
                        $('#outs').text(data.inplay.outs);
                        $('#fouls').text(data.inplay.fouls);
                        $('#runs').text(data.inplay.runs);
                        $('#runs2').text(data.inplay.runs2);
                        $('#inning').text(data.inplay.inningNumber);
                        $('#playerAtBat').text(data.inplay.firstPlayer);
                        $('#isVisitor').val(data.inplay.isVisitor);

                        // Check if isVisitor is true
                        if (!data.inplay.isVisitor) {
                            // Hide the buttons
                            $('#ballsButton, #strikeButton, #foulButton, #outButton, #hbpButton,#hitButton,#openPopupButton,#startButton,#inningButton,#lineupButton').hide();
                        } else {
                            // Show the buttons
                            $('#ballsButton, #strikeButton, #foulButton, #outButton, #hbpButton,#hitButton,#openPopupButton,#inningButton,#lineupButton').show();
                            $('#opponentScoreButton, #opponentOutsButton,#startButton,#lineupButton').hide();
                        }

                        // Actualiza las celdas de la tabla para el equipo "A"
                        var inningScoresTeam1 = data.inningScoresTeam1;
                        for (var key in inningScoresTeam1) {
                            if (key !== "$id") {  // Ignora la clave "$id"
                                var score = inningScoresTeam1[key];
                                console.log($('#inningScoreA' + key));
                                $('#inningScoreA' + key).text(score);
                            }
                        }

                        // Actualiza las celdas de la tabla para el equipo "H"
                        var inningScoresTeam2 = data.inningScoresTeam2;
                        for (var key in inningScoresTeam2) {
                            if (key !== "$id") {  // Ignora la clave "$id"
                                var score = inningScoresTeam2[key];
                                console.log($('#inningScoreH' + key));
                                $('#inningScoreH' + key).text(score);
                            }
                        }

                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        console.log(textStatus, errorThrown);
                    }
                });
            });


        //Exit Button
        $(document).ready(function () {
            $('.exit-game-option').click(function (e) {
                e.preventDefault();
                var exitType = $(this).text();
                var inplayId = parseInt($('#inplayId').val(), 10);

                // if user click on "End Game"
                if ($(this).text() === 'End Game') {
                    // shows an alert to confirm the action
                    if (confirm('Are you sure you want to end the game?')) {

                        $.ajax({
                            url: '/ScorePlayer/Exit',
                            type: 'POST',
                            data: { id: inplayId, exitType: exitType },
                            success: function (data) {
                                if (data.redirectTo) {
                                    window.location.href = data.redirectTo;
                                }
                            }
                        });
                    } else {

                    }
                }
                // if the user clicked "Cancel Game"
                if ($(this).text() === 'Cancel Game') {
                    // shows the alert to confirm the action
                    if (confirm('Are you sure you want to cancel this game?')) {

                        $.ajax({
                            url: '/ScorePlayer/Exit',
                            type: 'POST',
                            data: { id: inplayId, exitType: exitType },
                            success: function (data) {
                                if (data.redirectTo) {
                                    window.location.href = data.redirectTo;
                                }
                            }
                        });
                    } else {

                    }
                }


            });
        });

        //Hits Button
        $(document).ready(function () {
            $('.dropdown-item').click(function (e) {
                e.preventDefault();

                var hitType = $(this).text();
                var inplayId = parseInt($('#inplayId').val(), 10);

                $.ajax({
                    url: '/ScorePlayer/Hit',
                    type: 'POST',
                    data: { id: inplayId, hitType: hitType },
                    success: function (data) {
                        console.log(data);
                        if (data.success === false) {
                            // Show the error message in an alert
                            alert(data.message);
                        } else if (data.inplay) {
                            $('#inplayId').val(data.inplay.id);
                            $('#strikesCount').text(data.inplay.strikes);
                            $('#outs').text(data.inplay.outs);
                            $('#fouls').text(data.inplay.fouls);
                            $('#ballsCount').text(data.inplay.balls);
                            $('#runs').text(data.inplay.runs);
                            $('#runs2').text(data.inplay.runs2);
                            $('#inning').text(data.inplay.inningNumber);
                            $('#playerAtBat').text(data.inplay.firstPlayer);

                            // Actualiza las celdas de la tabla para el equipo "A"
                            var inningScoresTeam1 = data.inningScoresTeam1;
                            for (var key in inningScoresTeam1) {
                                if (key !== "$id") {  // Ignora la clave "$id"
                                    var score = inningScoresTeam1[key];
                                    console.log($('#inningScoreA' + key));
                                    $('#inningScoreA' + key).text(score);
                                }
                            }

                            // Actualiza las celdas de la tabla para el equipo "H"
                            var inningScoresTeam2 = data.inningScoresTeam2;
                            for (var key in inningScoresTeam2) {
                                if (key !== "$id") {  // Ignora la clave "$id"
                                    var score = inningScoresTeam2[key];
                                    console.log($('#inningScoreH' + key));
                                    $('#inningScoreH' + key).text(score);
                                }
                            }


                        } else {
                            console.log('inplay is undefined');
                        }

                        if (data.needUserDecision) {
                            swal({
                                title: data.message,
                                text: "Choose one option:",
                                icon: "warning",
                                buttons: {
                                    cancel: "Out",
                                    confirm: "Run"
                                }
                            }).then((isConfirm) => {
                                if (isConfirm) {
                                    // El usuario eligió "Run"
                                    handleUserDecision(inplayId, "Run", data.playerBatting, data.playerInBase3);
                                } else {
                                    // El usuario eligió "Out"
                                    handleUserDecision(inplayId, "Out", data.playerBatting, data.playerInBase3);
                                }
                            });
                        }
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        console.log(textStatus, errorThrown);
                    }
                });
            });
        });

        function handleUserDecision(inplayId, decision, batter, player) {

            $.ajax({
                url: '/ScorePlayer/HandleUserDecision',
                type: 'POST',
                data: { id: inplayId, decision: decision, batter: batter, player: player },
                success: function (data) {
                    console.log(data);
                    if (data.success === false) {
                        // Show the error message in an alert
                        alert(data.message);
                    } else if (data.inplay) {
                        $('#inplayId').val(data.inplay.id);
                        $('#strikesCount').text(data.inplay.strikes);
                        $('#outs').text(data.inplay.outs);
                        $('#fouls').text(data.inplay.fouls);
                        $('#ballsCount').text(data.inplay.balls);
                        $('#runs').text(data.inplay.runs);
                        $('#runs2').text(data.inplay.runs2);
                        $('#inning').text(data.inplay.inningNumber);
                        $('#playerAtBat').text(data.inplay.firstPlayer);

                        // Actualiza las celdas de la tabla para el equipo "A"
                        var inningScoresTeam1 = data.inningScoresTeam1;
                        for (var key in inningScoresTeam1) {
                            if (key !== "$id") {  // Ignora la clave "$id"
                                var score = inningScoresTeam1[key];
                                console.log($('#inningScoreA' + key));
                                $('#inningScoreA' + key).text(score);
                            }
                        }

                        // Actualiza las celdas de la tabla para el equipo "H"
                        var inningScoresTeam2 = data.inningScoresTeam2;
                        for (var key in inningScoresTeam2) {
                            if (key !== "$id") {  // Ignora la clave "$id"
                                var score = inningScoresTeam2[key];
                                console.log($('#inningScoreH' + key));
                                $('#inningScoreH' + key).text(score);
                            }
                        }


                    } else {
                        console.log('inplay is undefined');
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    console.log(textStatus, errorThrown);
                }
            });
        }
        //end hits button

        //Inning Button

        $('#inningButton').click(function (e) {
            e.preventDefault();
        var teamId = $(this).data('team-id');
        var gameId = $(this).data('game-id');
        var inplayId = $('#inplayId').val();
        console.log(teamId, gameId, inplayId);
        $.ajax({
            url: '/ScorePlayer/Inning',
        type: 'GET',
        data: {
            teamId: teamId,
        gameId: gameId,
        inplayId: inplayId
                },
        success: function (data) {
                    if (data.inplay) {
            $('#inplayId').val(data.inplay.id);
        $('#strikesCount').text(data.inplay.strikes);
        $('#outs').text(data.inplay.outs);
        $('#fouls').text(data.inplay.fouls);
        $('#ballsCount').text(data.inplay.balls);
        $('#runs').text(data.inplay.runs);
        $('#runs2').text(data.inplay.runs2);
        $('#inning').text(data.inplay.inningNumber);
        $('#playerAtBat').text(data.inplay.firstPlayer);
        $('#InningScore').text(data.inplay.inning);

        // Actualiza las celdas de la tabla para el equipo "A"
        var inningScoresTeam1 = data.inningScoresTeam1;
        for (var key in inningScoresTeam1) {
                            if (key !== "$id") {  // Ignora la clave "$id"
                                var score = inningScoresTeam1[key];
        console.log($('#inningScoreA' + key));
        $('#inningScoreA' + key).text(score);
                            }
                        }

        // Actualiza las celdas de la tabla para el equipo "H"
        var inningScoresTeam2 = data.inningScoresTeam2;
        for (var key in inningScoresTeam2) {
                            if (key !== "$id") {  // Ignora la clave "$id"
                                var score = inningScoresTeam2[key];
        console.log($('#inningScoreH' + key));
        $('#inningScoreH' + key).text(score);
                            }
                        }

                    } else {
            console.log('inplay is undefined');
                    }
                },
        error: function (jqXHR, textStatus, errorThrown) {
            console.log(textStatus, errorThrown);
                }
            });
        });


        // Ball Button

        $('#ballsButton').click(function (e) {
            e.preventDefault();
        var inplayId = $('#inplayId').val();

        $.ajax({
            url: '/ScorePlayer/CountBalls',
        type: 'POST',
        data: {inplayId: inplayId },
        success: function (data) {

                    if (data.success === false) {
            alert(data.message);
        return;
                    }
        $('#ballsCount').text(data.inplay.balls);
        $('#inplayId').val(data.inplay.id);
        $('#strikesCount').text(data.inplay.strikes);
        $('#outs').text(data.inplay.outs);
        $('#fouls').text(data.inplay.fouls);
        $('#runs').text(data.inplay.runs);
        $('#runs2').text(data.inplay.runs2);
        $('#inning').text(data.inplay.inningNumber);
        $('#playerAtBat').text(data.inplay.firstPlayer);


        // Actualiza las celdas de la tabla para el equipo "A"
        var inningScoresTeam1 = data.inningScoresTeam1;
        for (var key in inningScoresTeam1) {
                        if (key !== "$id") {  // Ignora la clave "$id"
                            var score = inningScoresTeam1[key];
        console.log($('#inningScoreA' + key));
        $('#inningScoreA' + key).text(score);
                        }
                    }

        // Actualiza las celdas de la tabla para el equipo "H"
        var inningScoresTeam2 = data.inningScoresTeam2;
        for (var key in inningScoresTeam2) {
                        if (key !== "$id") {  // Ignora la clave "$id"
                            var score = inningScoresTeam2[key];
        console.log($('#inningScoreH' + key));
        $('#inningScoreH' + key).text(score);
                        }
                    }

                },
        error: function (jqXHR, textStatus, errorThrown) {
            console.log(textStatus, errorThrown);
                }
            });
        });
        // Strike Button

        $('#strikeButton').click(function (e) {
            e.preventDefault();
        var inplayId = $('#inplayId').val();;

        $.ajax({
            url: '/ScorePlayer/CountStrikes',
        type: 'POST',
        data: {inplayId: inplayId },
        success: function (data) {
                    if (data.success === false) {
            alert(data.message);
        return;
                    }
        // Actualiza el contador de Balls en la vista
        $('#ballsCount').text(data.inplay.balls);
        $('#inplayId').val(data.inplay.id);
        $('#strikesCount').text(data.inplay.strikes);
        $('#outs').text(data.inplay.outs);
        $('#fouls').text(data.inplay.fouls);
        $('#runs').text(data.inplay.runs);
        $('#runs2').text(data.inplay.runs2);
        $('#inning').text(data.inplay.inningNumber);
        $('#playerAtBat').text(data.inplay.firstPlayer);
        $('#isVisitor').val(data.inplay.isVisitor);

        // Check if isVisitor is true
            if (data.inplay.isVisitor)
            {
                // Hide the buttons
                $('#opponentScoreButton, #opponentOutsButton').hide();
                $('#ballsButton, #strikeButton, #foulButton, #outButton, #hbpButton,#hitButton,#openPopupButton,#inningButton,#lineupButton').show();
            }
            else
            {
                // Show the buttons
                $('#ballsButton, #strikeButton, #foulButton, #outButton, #hbpButton,#hitButton,#openPopupButton,#inningButton,#lineupButton').hide();
                $('#opponentScoreButton, #opponentOutsButton').show();
            }
                },
        error: function (jqXHR, textStatus, errorThrown) {
            console.log(textStatus, errorThrown);
                }
            });
        });

        // Foul Button

        $('#foulButton').click(function (e) {
            e.preventDefault();
        var inplayId = $('#inplayId').val();;

        $.ajax({
            url: '/ScorePlayer/CountFouls',
        type: 'POST',
        data: {inplayId: inplayId },
        success: function (data) {
                    if (data.success === false) {
            alert(data.message);
        return;
                    }
        // Actualiza el contador de Balls en la vista
        $('#ballsCount').text(data.inplay.balls);
        $('#inplayId').val(data.inplay.id);
        $('#strikesCount').text(data.inplay.strikes);
        $('#outs').text(data.inplay.outs);
        $('#fouls').text(data.inplay.fouls);
        $('#runs').text(data.inplay.runs);
        $('#runs2').text(data.inplay.runs2);
        $('#inning').text(data.inplay.inningNumber);
        $('#playerAtBat').text(data.inplay.firstPlayer);
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.log(textStatus, errorThrown);
                }
            });
        });

        // Hit by pitch Button

        $('#hbpButton').click(function (e) {
            e.preventDefault();
        var inplayId = $('#inplayId').val();

        $.ajax({
            url: '/ScorePlayer/HitByPitch',
        type: 'POST',
        data: {inplayId: inplayId },
        success: function (data) {
                    if (data.success === false) {
            alert(data.message);
        return;
                    }
        // Actualiza el contador de Balls en la vista
        $('#ballsCount').text(data.inplay.balls);
        $('#inplayId').val(data.inplay.id);
        $('#strikesCount').text(data.inplay.strikes);
        $('#outs').text(data.inplay.outs);
        $('#fouls').text(data.inplay.fouls);
        $('#runs').text(data.inplay.runs);
        $('#runs2').text(data.inplay.runs2);
        $('#inning').text(data.inplay.inningNumber);
        $('#playerAtBat').text(data.inplay.firstPlayer);

        // Actualiza las celdas de la tabla para el equipo "A"
        var inningScoresTeam1 = data.inningScoresTeam1;
        for (var key in inningScoresTeam1) {
                        if (key !== "$id") {  // Ignora la clave "$id"
                            var score = inningScoresTeam1[key];
        console.log($('#inningScoreA' + key));
        $('#inningScoreA' + key).text(score);
                        }
                    }

        // Actualiza las celdas de la tabla para el equipo "H"
        var inningScoresTeam2 = data.inningScoresTeam2;
        for (var key in inningScoresTeam2) {
                        if (key !== "$id") {  // Ignora la clave "$id"
                            var score = inningScoresTeam2[key];
        console.log($('#inningScoreH' + key));
        $('#inningScoreH' + key).text(score);
                        }
                    }

                },
        error: function (jqXHR, textStatus, errorThrown) {
            console.log(textStatus, errorThrown);
                }
            });
        });

        // Out Button

        $('#outButton').click(function (e) {
            e.preventDefault();
        var inplayId = $('#inplayId').val();;

        $.ajax({
            url: '/ScorePlayer/Out',
        type: 'POST',
        data: {inplayId: inplayId },
        success: function (data) {
                    if (data.success === false) {
            alert(data.message);
        return;
                    }
        // Actualiza el contador de Balls en la vista
        $('#ballsCount').text(data.inplay.balls);
        $('#inplayId').val(data.inplay.id);
        $('#strikesCount').text(data.inplay.strikes);
        $('#outs').text(data.inplay.outs);
        $('#fouls').text(data.inplay.fouls);
        $('#runs').text(data.inplay.runs);
        $('#runs2').text(data.inplay.runs2);
        $('#inning').text(data.inplay.inningNumber);
        $('#playerAtBat').text(data.inplay.firstPlayer);
        $('#inningScore').text(data.inplay.inningScore);
        $('#isVisitor').val(data.inplay.isVisitor);

            // Check if isVisitor is true
            if (data.inplay.isVisitor) {
                // Hide the buttons
                $('#opponentScoreButton, #opponentOutsButton').hide();
                $('#ballsButton, #strikeButton, #foulButton, #outButton, #hbpButton,#hitButton,#openPopupButton,#inningButton,#lineupButton').show();
            } else {
                // Show the buttons
                $('#ballsButton, #strikeButton, #foulButton, #outButton, #hbpButton,#hitButton,#openPopupButton,#inningButton,#lineupButton').hide();
                $('#opponentScoreButton, #opponentOutsButton').show();
            }


                },
        error: function (jqXHR, textStatus, errorThrown) {
            console.log(textStatus, errorThrown);
                }
            });
        });


    });
 
  /*  Script for the LineUp Button */


            $(document).ready(function () {
                // Existing code for the start button AJAX request goes here
                $('#popoverButton').popover({
                    container: 'body',
                    content: $('#popoverContent').html(),
                    html: true,
                    title: 'Popover Title',
                });

            $('#lineupButton').click(function (e) {
                e.preventDefault();

            var gameId = $(this).data('gameid');
            var teamId = $(this).data('teamid');

            $.ajax({
                url: '/ScorePlayer/GetLineup', // Change the URL to your endpoint that returns the lineup view
            type: 'GET',
            data: {gameId: gameId, teamId: teamId },
            success: function (data) {
                $('#lineupPopup .popup-content').html(data);
            $('#lineupPopup').show();
                    },
            error: function () {
                console.error('Failed to fetch lineup data.');
                    }
                });
            });

            $(document).click(function (e) {
                if (!$(e.target).closest('#lineupPopup').length) {
                $('#lineupPopup').hide();
                }
            });
        });
        
    
//Showing alert for each button
    
    $('#ballsButton, #strikeButton, #foulButton, #outButton, #hbpButton, #opponentScoreButton, #opponentOutsButton').click(function (e) {
        e.preventDefault();

        // get id of each button
        var buttonId = $(this).attr('id');
        var buttonText = $(this).find('span').text();

        // if the button is #opponentScoreButton o #opponentOutsButton, change the text
        if (buttonId === 'opponentScoreButton') {
            buttonText = 'Run';
        } else if (buttonId === 'opponentOutsButton') {
            buttonText = 'Out';
        }

        //change the text and show the alert
        $('#alert').text(buttonText).fadeIn().delay(1500).fadeOut();
    });


});