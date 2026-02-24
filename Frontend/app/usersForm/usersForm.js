const API_BASE_URL = "http://localhost:17948/api/users";

function initializeUserForm() {
  let submitButton = document.querySelector("#saveUserBtn");
  submitButton.addEventListener("click", saveUser);

  let cancelButton = document.querySelector("#cancelBtn");
  cancelButton.addEventListener("click", function () {
    window.location.href = "../users/users.html";
  });

  const urlParams = new URLSearchParams(window.location.search);
  const id = urlParams.get("id");
  if (!id) {
    document.getElementById("addTitle").style.display = "block";
    document.getElementById("editTitle").style.display = "none";
    return;
  }

  document.getElementById("addTitle").style.display = "none";
  document.getElementById("editTitle").style.display = "block";
  get(id);
}

function get(id) {
  fetch(API_BASE_URL + "/" + id)
    .then((response) => {
      if (!response.ok) {
        const error = new Error("Request failed. Status: " + response.status);
        error.response = response;
        throw error;
      }
      return response.json();
    })
    .then((user) => {
      document.querySelector("#username").value = user.username;
      document.querySelector("#firstName").value = user.firstName;
      document.querySelector("#lastName").value = user.lastName;
      const formattedDate = new Date(user.dateOfBirth)
        .toISOString()
        .split("T")[0];
      document.querySelector("#dateOfBirth").value = formattedDate;
    })
    .catch((error) => {
      console.error("Error:", error.message);
      if (error.response && error.response.status === 404) {
        alert("User not found.");
      } else {
        alert("An error occurred while fetching the user.");
      }
    });
}
function saveUser() {
  const form = document.querySelector("#userForm");
  const formData = new FormData(form);

  const reqBody = {
    username: formData.get("username"),
    firstName: formData.get("firstName"),
    lastName: formData.get("lastName"),
    dateOfBirth: new Date(formData.get("dateOfBirth")),
  };
  const usernameErrorMessage = document.querySelector("#usernameError");
  usernameErrorMessage.textContent = "";
  const firstNameErrorMessage = document.querySelector("#firstNameError");
  firstNameErrorMessage.textContent = "";
  const lastNameErrorMessage = document.querySelector("#lastNameError");
  lastNameErrorMessage.textContent = "";
  const dateOfBirthErrorMessage = document.querySelector("#dateOfBirthError");
  dateOfBirthErrorMessage.textContent = "";

  if (reqBody.username.trim() === "") {
    usernameErrorMessage.textContent = "Username is required";
    return;
  }
  if (reqBody.firstName.trim() === "") {
    firstNameErrorMessage.textContent = "First name is required";
    return;
  }
  if (reqBody.lastName.trim() === "") {
    lastNameErrorMessage.textContent = "Last name is required";
    return;
  }
  if (reqBody.dateOfBirth === "") {
    dateOfBirthErrorMessage.textContent = "Date of birth is required";
    return;
  }

  let method = "POST";
  let url = API_BASE_URL;

  const urlParams = new URLSearchParams(window.location.search);
  const id = urlParams.get("id");
  if (id) {
    method = "PUT";
    url += "/" + id;

  }
  fetch(url, {
    method: method,
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(reqBody),
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
      window.location.href = "../users/users.html";
    })
    .catch((error) => {
      console.error("Error:", error.message);
      if (error.response && error.response.status === 404) {
        alert("User not found.");
      } else if (error.response && error.response.status === 400) {
        alert("Data is invalid.");
      } else {
        alert("An error occurred while updating the data. Please try again.");
      }
    });
}

document.addEventListener("DOMContentLoaded", initializeUserForm);
