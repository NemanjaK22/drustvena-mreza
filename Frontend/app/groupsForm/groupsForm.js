const API_BASE_URL = "http://localhost:17948/api/groups";

function initializeAddGroup() {
  let saveBtn = document.querySelector("#saveGroupBtn");
  let cancelBtn = document.querySelector("#cancelBtn");

  saveBtn.addEventListener("click", saveNewGroup);

  cancelBtn.addEventListener("click", function () {
    window.location.href = "../groups/groups.html";
  });
}

function saveNewGroup() {
  const form = document.querySelector("#addGroupForm");
  const formData = new FormData(form);
  const nameError = document.querySelector("#nameError");

  const newGroup = {
    name: formData.get("name"),
    createdDate: new Date().toISOString().split("T")[0],
  };

  if (nameError) nameError.textContent = "";

  if (!newGroup.name || newGroup.name.trim() === "") {
    if (nameError) nameError.textContent = "Name field is required.";
    return;
  }

  fetch(API_BASE_URL, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(newGroup),
  })
    .then((response) => {
      if (!response.ok) {
        const error = new Error("Request failed. Status: " + response.status);
        error.response = response;
        throw error;
      }
      return response.json();
    })
    .then((data) => {
      window.location.href = "../groups/groups.html";
    })
    .catch((error) => {
      console.error("Error:", error.message);
      if (error.response && error.response.status === 400) {
        if (nameError) nameError.textContent = "Invalid data sent to server.";
      } else {
        console.error("An error occurred while communicating with the server.");
      }
    });
}

document.addEventListener("DOMContentLoaded", initializeAddGroup);
