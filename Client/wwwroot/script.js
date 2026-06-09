const apiBaseUrl = "http://localhost:5213/api";

const eventsContainer = document.getElementById("eventsContainer");
const eventDetailsContainer = document.getElementById("eventDetailsContainer");
const statisticsContainer = document.getElementById("statisticsContainer");
const updateEventSelect = document.getElementById("updateEventSelect");
const deleteEventSelect = document.getElementById("deleteEventSelect");
const myScheduleContainer = document.getElementById("myScheduleContainer");

let adminEvents = [];

// If we are in index.html, load the dashboard
if (eventsContainer !== null) {
    loadEvents();
}
// If we are in eventDetails.html, load the selected event details
if (eventDetailsContainer !== null) {
    loadEventDetailsPage();
}
// If we are in statistics.html, load the statistics
if (statisticsContainer !== null) {
    loadStatistics();
}
// If we are in admin.html, load the admin's options
if (updateEventSelect !== null && deleteEventSelect !== null) {
    loadAdminEvents();
}
// If we are in mySchedule.html, load the user's schedule'
if (myScheduleContainer !== null) {
    myScheduleContainer.innerHTML = "";
}

function loadEvents() {
    eventsContainer.innerHTML = "<p>Loading events...</p>";

    fetch(apiBaseUrl + "/event/schedule").then(response => {
            if (!response.ok) {
                throw new Error("Failed to load events");
            }

            return response.json();
        }).then(events => {
            eventsContainer.innerHTML = "";

            if (events.length === 0) {
                eventsContainer.innerHTML = "<p>No events found.</p>";
                return;
            }

            events.forEach(event => {
                const eventCard = document.createElement("div");
                eventCard.className = "event-card";

                eventCard.innerHTML = `
                    <h2>${event.title}</h2>
                    <p><strong>Date:</strong> ${new Date(event.startDate).toLocaleDateString()}</p>
                    <p><strong>Location:</strong> ${event.location}</p>
                    <p><strong>Type:</strong> ${event.eventType}</p>
                    <p><strong>Sessions:</strong> ${event.sessions.length}</p>

                    <button onclick="viewEventDetails(${event.id})">View Details</button>
                `;

                eventsContainer.appendChild(eventCard);
            });
        }).catch(error => {
            eventsContainer.innerHTML = "<p class='error-message'>Trying to connect to server...</p>";
            console.log(error);

            setTimeout(() => {
                loadEvents();
            }, 1000);
        });
}

function viewEventDetails(eventId) {
    window.location.href = "eventDetails.html?id=" + eventId;
}

function loadEventDetailsPage() {
    const urlParams = new URLSearchParams(window.location.search);
    const eventId = urlParams.get("id");

    if (eventId === null) {
        eventDetailsContainer.innerHTML = `<p class="error-message">Event id was not found</p>`;
        return;
    }

    fetch(apiBaseUrl + "/event/" + eventId)
        .then(response => {
            if (!response.ok) {
                throw new Error("Failed to load event details");
            }

            return response.json();
        })
        .then(event => {
            eventDetailsContainer.innerHTML = `
                <div class="details-card">
                    <h2>${event.title}</h2>

                    <p><strong>Description:</strong> ${event.description}</p>
                    <p><strong>Start:</strong> ${new Date(event.startDate).toLocaleString()}</p>
                    <p><strong>End:</strong> ${new Date(event.endDate).toLocaleString()}</p>
                    <p><strong>Location:</strong> ${event.location}</p>
                    <p><strong>Type:</strong> ${event.eventType}</p>

                    <div id="weatherContainer">
                        <p>Loading weather...</p>
                    </div>

                    <h3>Sessions</h3>
                    <div id="sessionsContainer"></div>
                </div>
            `;

            displaySessions(event.sessions);
            loadWeather(event.id);
        })
        .catch(error => {
            eventDetailsContainer.innerHTML = `<p class="error-message">${error.message}</p>`;
            console.log(error);
        });
}

function displaySessions(sessions) {
    const sessionsContainer = document.getElementById("sessionsContainer");

    if (sessions.length === 0) {
        sessionsContainer.innerHTML = "<p>No sessions for this event.</p>";
        return;
    }

    sessions.sort((a, b) => new Date(a.startTime) - new Date(b.startTime));

    sessionsContainer.innerHTML = "";

    sessions.forEach(session => {
        const sessionDiv = document.createElement("div");
        sessionDiv.className = "session-card";

        sessionDiv.innerHTML = `
            <h4>${session.title}</h4>
            <p><strong>Description:</strong> ${session.description}</p>
            <p><strong>Speaker:</strong> ${session.speakerName}</p>
            <p><strong>Start:</strong> ${new Date(session.startTime).toLocaleString()}</p>
            <p><strong>End:</strong> ${new Date(session.endTime).toLocaleString()}</p>
            <p><strong>Room:</strong> ${session.roomName}</p>

            <div class="register-area">
                <p><strong>Register to this session:</strong></p>
                <label>User Id:</label>
                <input type="number" id="userId-${session.id}" placeholder="Enter user id">
                <button onclick="registerToSession(${session.id})">Register</button>
                <p id="registerMessage-${session.id}"></p>
            </div>
        `;

        sessionsContainer.appendChild(sessionDiv);
    });
}

function loadWeather(eventId) {
    const weatherContainer = document.getElementById("weatherContainer");

    fetch(apiBaseUrl + "/event/" + eventId + "/weather").then(response => {
            if (!response.ok) {
                throw new Error("Failed to load weather");
            }

            return response.json();
        }).then(weather => {
            const weatherText = weather.weatherInfo;

            weatherContainer.innerHTML = `
        <h3>Current Weather at Event Location</h3>

        <div class="weather-box">
       
            <div class="weather-info">
                <div class="weather-item">
                    <span class="weather-title">Weather</span>
                    <span>${getWeatherPart(weatherText, "Weather:", ", Temperature:")}</span>
                </div>

                <div class="weather-item">
                    <span class="weather-title">Temperature</span>
                    <span>${getWeatherPart(weatherText, "Temperature:", "°C") + "°C"}</span>
                </div>

                <div class="weather-item">
                    <span class="weather-title">Feels Like</span>
                    <span>${getWeatherPart(weatherText, "Feels like:", "°C") + "°C"}</span>
                </div>

                <div class="weather-item">
                    <span class="weather-title">Humidity</span>
                    <span>${getWeatherPart(weatherText, "Humidity:", "%") + "%"}</span>
                </div>
            </div>
        </div>
    `;
        }).catch(error => {
            weatherContainer.innerHTML = `<p class="error-message">${error.message}</p>`;
            console.log(error);
        });
}

function getWeatherPart(text, startText, endText) {
    const startIndex = text.indexOf(startText);

    if (startIndex === -1) {
        return "";
    }
    const realStart = startIndex + startText.length;
    const endIndex = text.indexOf(endText, realStart);
    if (endIndex === -1) {
        return text.substring(realStart).trim();
    }
    return text.substring(realStart, endIndex).trim();
}

function registerToSession(sessionId) {
    const userIdInput = document.getElementById("userId-" + sessionId);
    const message = document.getElementById("registerMessage-" + sessionId);

    const userId = Number(userIdInput.value);

    message.innerHTML = "";
    message.className = "";
    if (userId === 0) {
        message.innerHTML = "Please enter user id";
        message.className = "error-message";
        setTimeout(() => {
            message.innerHTML = "";
            message.className = "";
        }, 4000);
        return;
    }

    fetch(apiBaseUrl + "/session/" + sessionId + "/register", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({
            userId: userId
        })
    }).then(response => {
            if (!response.ok) {
                return response.text().then(text => {
                    throw new Error(text);
                });
            }

            return response.text();
        }).then(result => {
            message.innerHTML = result;
            message.className = "success-message";
            userIdInput.value = ""; // clean the input after success

            setTimeout(() => {
                message.innerHTML = "";
                message.className = "";
            }, 4000);
        }).catch(error => {
            message.innerHTML = error.message;
            message.className = "error-message";
            userIdInput.value = ""; // clean the input after error
        console.log(error);

            setTimeout(() => {
                message.innerHTML = "";
                message.className = "";
            }, 4000);
        });
}

function goBackToDashboard() {
    window.location.href = "index.html";
}

// Statistics
// Statistics
async function loadStatistics() {
    statisticsContainer.innerHTML = "<p>Loading statistics...</p>";

    try {
        const response = await fetch(apiBaseUrl + "/event/schedule");

        if (!response.ok) {
            throw new Error("Failed to load statistics");
        }

        const events = await response.json();

        if (events.length === 0) {
            statisticsContainer.innerHTML = "<p>No events found.</p>";
            return;
        }

        let totalEvents = events.length;
        let totalSessions = 0;
        let totalParticipants = 0;
        let eventWithMostSessions = events[0];

        for (const event of events) {
            totalSessions += event.sessions.length;

            if (event.sessions.length > eventWithMostSessions.sessions.length) {
                eventWithMostSessions = event;
            }

            for (const session of event.sessions) {
                const usersResponse = await fetch(apiBaseUrl + "/session/" + session.id + "/user");

                if (usersResponse.ok) {
                    const users = await usersResponse.json();
                    totalParticipants += users.length;
                }
            }
        }

        let averageSessions = totalSessions / totalEvents;
        let averageParticipants = 0;

        if (totalSessions > 0) {
            averageParticipants = totalParticipants / totalSessions;
        }

        let today = new Date();
        let upcomingEvents = events.filter(event => {
            return new Date(event.startDate) >= today;
        });

        let eventsByMonth = {};

        events.forEach(event => {
            let eventDate = new Date(event.startDate);

            let monthName = eventDate.toLocaleString("en-US", {
                month: "long",
                year: "numeric"
            });

            if (eventsByMonth[monthName] === undefined) {
                eventsByMonth[monthName] = 1;
            } else {
                eventsByMonth[monthName]++;
            }
        });

        statisticsContainer.innerHTML = `
            <div class="statistics-grid">
                <div class="stat-card">
                    <h2>Total Events</h2>
                    <p>${totalEvents}</p>
                </div>

                <div class="stat-card">
                    <h2>Total Sessions</h2>
                    <p>${totalSessions}</p>
                </div>

                <div class="stat-card">
                    <h2>Average Participants per session</h2>
                    <p>${averageParticipants.toFixed(1)}</p>
                </div>

                <div class="stat-card">
                    <h2>Upcoming Events</h2>
                    <p>${upcomingEvents.length}</p>
                </div>
            </div>

            <div class="statistics-section">
                <h2>Event With Most Sessions</h2>
                <p><strong>${eventWithMostSessions.title}</strong></p>
                <p>Sessions: ${eventWithMostSessions.sessions.length}</p>
            </div>

            <div class="statistics-section">
                <h2>Additional Statistics</h2>
                <p><strong>Average Sessions Per Event:</strong> ${averageSessions.toFixed(1)}</p>
                <p><strong>Total Registrations:</strong> ${totalParticipants}</p>
            </div>
            
            <div class="statistics-section">
                <h2>Events By Month</h2>
                <ul id="eventsByMonthList"></ul>
            </div>

            <div class="statistics-section">
                <h2>Upcoming Events List</h2>
                <ul id="upcomingEventsList"></ul>
            </div>
        `;

        const upcomingEventsList = document.getElementById("upcomingEventsList");
        const eventsByMonthList = document.getElementById("eventsByMonthList");

        for (let month in eventsByMonth) {
            const li = document.createElement("li");
            li.innerHTML = `<strong>${month}</strong> - ${eventsByMonth[month]} events`;
            eventsByMonthList.appendChild(li);
        }

        if (upcomingEvents.length === 0) {
            upcomingEventsList.innerHTML = "<li>No upcoming events.</li>";
        } else {
            upcomingEvents.forEach(event => {
                const li = document.createElement("li");
                li.innerHTML = `
                    <strong>${event.title}</strong> -
                    ${new Date(event.startDate).toLocaleDateString()} -
                    ${event.location}
                `;
                upcomingEventsList.appendChild(li);
            });
        }
    } catch (error) {
        statisticsContainer.innerHTML = "<p class='error-message'>Failed to load statistics</p>";
        console.log(error);
    }
}

// Admin login
function adminLogin() {
    const usernameInput = document.getElementById("adminUsername");
    const passwordInput = document.getElementById("adminPassword");
    const message = document.getElementById("adminLoginMessage");

    const username = usernameInput.value;
    const password = passwordInput.value;

    message.innerHTML = "";
    message.className = "";

    if (username === "admin" && password === "1234") {
        message.innerHTML = "Login successful";
        message.className = "success-message";

        setTimeout(() => {
            window.location.href = "Admin.html";
        }, 700);
    } else {
        message.innerHTML = "Invalid username or password";
        message.className = "error-message";

        usernameInput.value = "";
        passwordInput.value = "";
    }
}

// Admin panel
function loadAdminEvents() {
    fetch(apiBaseUrl + "/event/schedule").then(response => {
            if (!response.ok) {
                throw new Error("Failed to load admin events");
            }
            return response.json();
        }).then(events => {
            adminEvents = events;

            updateEventSelect.innerHTML = `<option value="">Choose event</option>`;
            deleteEventSelect.innerHTML = `<option value="">Choose event</option>`;

            events.forEach(event => {
                const updateOption = document.createElement("option");
                updateOption.value = event.id;
                updateOption.text = event.title;
                updateEventSelect.appendChild(updateOption);

                const deleteOption = document.createElement("option");
                deleteOption.value = event.id;
                deleteOption.text = event.title;
                deleteEventSelect.appendChild(deleteOption);
            });
        }).catch(error => {
            console.log(error);
        });
}

function AdminCreateEvent() {
    const message = document.getElementById("createEventMessage");

    const newEvent = {
        title: document.getElementById("createTitle").value,
        description: document.getElementById("createDescription").value,
        startDate: document.getElementById("createStartDate").value,
        endDate: document.getElementById("createEndDate").value,
        location: document.getElementById("createLocation").value,
        eventType: document.getElementById("createEventType").value
    };

    message.innerHTML = "";
    message.className = "";

    if (newEvent.title === "" || newEvent.startDate === "" || newEvent.endDate === "" || newEvent.location === "" || newEvent.eventType === "") {
        message.innerHTML = "Please fill all required fields";
        message.className = "error-message";
        return;
    }

    fetch(apiBaseUrl + "/event", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(newEvent)
    }).then(response => {
            if (!response.ok) {
                return response.text().then(text => {
                    throw new Error(text);
                });
            }

            return response.text();
        }).then(result => {
            message.innerHTML = "Event created successfully";
            message.className = "success-message";

            clearCreateForm();
            loadAdminEvents();
        }).catch(error => {
            message.innerHTML = error.message;
            message.className = "error-message";
            console.log(error);
        });
}

function clearCreateForm() {
    document.getElementById("createTitle").value = "";
    document.getElementById("createDescription").value = "";
    document.getElementById("createStartDate").value = "";
    document.getElementById("createEndDate").value = "";
    document.getElementById("createLocation").value = "";
    document.getElementById("createEventType").value = "";
}

function fillUpdateForm() {
    const selectedEventId = Number(updateEventSelect.value);
    if (selectedEventId === 0) {
        document.getElementById("updateTitle").value = "";
        document.getElementById("updateDescription").value = "";
        document.getElementById("updateStartDate").value = "";
        document.getElementById("updateEndDate").value = "";
        document.getElementById("updateLocation").value = "";
        document.getElementById("updateEventType").value = "";
        return;
    }
    const selectedEvent = adminEvents.find(event => event.id === selectedEventId);

    if (selectedEvent === undefined) {
        return;
    }
    document.getElementById("updateTitle").value = selectedEvent.title;
    document.getElementById("updateDescription").value = selectedEvent.description;
    document.getElementById("updateStartDate").value = convertToDateTimeLocal(selectedEvent.startDate);
    document.getElementById("updateEndDate").value = convertToDateTimeLocal(selectedEvent.endDate);
    document.getElementById("updateLocation").value = selectedEvent.location;
    document.getElementById("updateEventType").value = selectedEvent.eventType;
}

function updateEvent() {
    const eventId = Number(updateEventSelect.value);
    const message = document.getElementById("updateEventMessage");

    message.innerHTML = "";
    message.className = "";

    if (eventId === 0) {
        message.innerHTML = "Please choose an event";
        message.className = "error-message";
        return;
    }

    const updatedEvent = {
        title: document.getElementById("updateTitle").value,
        description: document.getElementById("updateDescription").value,
        startDate: document.getElementById("updateStartDate").value,
        endDate: document.getElementById("updateEndDate").value,
        location: document.getElementById("updateLocation").value,
        eventType: document.getElementById("updateEventType").value
    };

    if (updatedEvent.title === "" || updatedEvent.startDate === "" || updatedEvent.endDate === "" || updatedEvent.location === "" || updatedEvent.eventType === "") {
        message.innerHTML = "Please fill all required fields";
        message.className = "error-message";
        return;
    }

    fetch(apiBaseUrl + "/event/" + eventId, {
        method: "PUT",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(updatedEvent)
    }).then(response => {
            if (!response.ok) {
                return response.text().then(text => {
                    throw new Error(text);
                });
            }

            return response.json();
        }).then(result => {
            message.innerHTML = "Event updated successfully";
            message.className = "success-message";

            clearUpdateForm();
            loadAdminEvents();
        }).catch(error => {
            message.innerHTML = error.message;
            message.className = "error-message";
            console.log(error);
        });
}

function clearUpdateForm() {
    updateEventSelect.value = "";
    document.getElementById("updateTitle").value = "";
    document.getElementById("updateDescription").value = "";
    document.getElementById("updateStartDate").value = "";
    document.getElementById("updateEndDate").value = "";
    document.getElementById("updateLocation").value = "";
    document.getElementById("updateEventType").value = "";
}

function deleteEvent() {
    const eventId = Number(deleteEventSelect.value);
    const message = document.getElementById("deleteEventMessage");

    message.innerHTML = "";
    message.className = "";

    if (eventId === 0) {
        message.innerHTML = "Please choose an event";
        message.className = "error-message";
        return;
    }
    const approved = confirm("Are you sure you want to delete this event?");
    if (!approved) {
        return;
    }

    fetch(apiBaseUrl + "/event/" + eventId, {
        method: "DELETE"
    }).then(response => {
            if (!response.ok) {
                return response.text().then(text => {
                    throw new Error(text);
                });
            }

            return response.text();
        }).then(result => {
            message.innerHTML = result;
            message.className = "success-message";

            loadAdminEvents();
        }).catch(error => {
            message.innerHTML = error.message;
            message.className = "error-message";
            console.log(error);
        });
}

function convertToDateTimeLocal(dateTime) {
    return dateTime.substring(0, 16);
}

// My Schedule
function loadUserSchedule() {
    const userIdInput = document.getElementById("scheduleUserId");
    const message = document.getElementById("scheduleMessage");

    const userId = Number(userIdInput.value);

    message.innerHTML = "";
    message.className = "";
    myScheduleContainer.innerHTML = "";

    if (userId === 0) {
        message.innerHTML = "Please enter user id";
        message.className = "error-message";
        return;
    }

    myScheduleContainer.innerHTML = "<p>Loading schedule...</p>";

    fetch(apiBaseUrl + "/user/" + userId + "/schedule").then(response => {
            if (!response.ok) {
                return response.text().then(text => {
                    throw new Error(text);
                });
            }

            return response.json();
        }).then(sessions => {
            myScheduleContainer.innerHTML = "";

            if (sessions.length === 0) {
                myScheduleContainer.innerHTML = `
                    <div class="statistics-section">
                        <h2>No Registered Sessions</h2>
                        <p>This user is not registered to any sessions yet.</p>
                    </div>
                `;
                return;
            }

            sessions.sort((a, b) => new Date(a.startTime) - new Date(b.startTime));

            myScheduleContainer.innerHTML = `
                <div class="statistics-section">
                    <h2>Registered Sessions</h2>
                    <p><strong>User Id:</strong> ${userId}</p>
                    <div id="registeredSessionsList"></div>
                </div>
            `;

            const registeredSessionsList = document.getElementById("registeredSessionsList");

            sessions.forEach(session => {
                const sessionCard = document.createElement("div");
                sessionCard.className = "session-card";

                sessionCard.innerHTML = `
                    <h4>${session.title}</h4>
                    <p><strong>Description:</strong> ${session.description}</p>
                    <p><strong>Speaker:</strong> ${session.speakerName}</p>
                    <p><strong>Start:</strong> ${new Date(session.startTime).toLocaleString()}</p>
                    <p><strong>End:</strong> ${new Date(session.endTime).toLocaleString()}</p>
                    <p><strong>Room:</strong> ${session.roomName}</p>
                    <p><strong>Registered At:</strong> ${session.registrationDate === null ? "Not available" : new Date(session.registrationDate).toLocaleString()}</p>
                `;

                registeredSessionsList.appendChild(sessionCard);
            });
        }).catch(error => {
            myScheduleContainer.innerHTML = "";
            message.innerHTML = error.message;
            message.className = "error-message";
            console.log(error);
        });
}