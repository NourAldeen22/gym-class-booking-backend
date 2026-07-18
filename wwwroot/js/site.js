// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

async function getData(id) {
  try {
    const response = await fetch(`/GymClass/Details/${id}`);
    if (!response.ok) {
      throw new Error("Network Error");
    }
    const data = await response.json();
    console.log(data);

    const list = document.getElementById("memberList");
    list.innerHTML = "";
    if (data.length === 0) {
      list.innerHTML = "<li>No Member yet</li>";
    }

    data.forEach((name) => {
      let li = document.createElement("li");
      li.className = "list-group-item";
      li.textContent = name;
      list.appendChild(li);
    });

    document.getElementById("detailsSection").style.display = "block";
  } catch (error) {
    console.error("Error", error);
  }
}

getData();

// const togglePassword = document.querySelectorAll("#togglePassword");
// const passwordInput = document.querySelectorAll("#passwordInput");
// const eyeIcon = document.querySelectorAll("#eyeIcon");

// togglePassword.forEach((toggle) => {
//   addEventListener("click", function () {
//     // التبديل بين نوع password ونوع text
//     const type =
//       passwordInput.getAttribute("type") === "password" ? "text" : "password";
//     passwordInput.setAttribute("type", type);

//     // تغيير شكل الأيقونة (عين مفتوحة / عين مغلقة)
//     this.classList.toggle("active");
//     if (type === "text") {
//       eyeIcon.classList.remove("bi-eye");
//       eyeIcon.classList.add("bi-eye-slash"); // أيقونة العين المشطوبة
//     } else {
//       eyeIcon.classList.remove("bi-eye-slash");

//       eyeIcon.classList.add("bi-eye"); // عين العادية
//     }
//   });

//   document
//     .querySelectorAll("#btn-delete")
//     .addEventListener("click", function (e) {
//       e.preventDefault();
//       const modal = document.querySelectorAll(".delete-modal");

//       modal.classList.toggle(modal);
//     });
// });

const togglePassword = document.querySelectorAll(".togglePassword");

togglePassword.forEach((toggle) => {
  toggle.addEventListener("click", function () {
    const container = this.closest(".password-container");

    const passwordInput = container.querySelector(".passwordInput");
    const eyeIcon = container.querySelector(".eyeIcon");

    const type =
      passwordInput.getAttribute("type") === "password" ? "text" : "password";
    passwordInput.setAttribute("type", type);

    if (type === "text") {
      eyeIcon.classList.remove("bi-eye");
      eyeIcon.classList.add("bi-eye-slash");
    } else {
      eyeIcon.classList.remove("bi-eye-slash");
      eyeIcon.classList.add("bi-eye");
    }
  });
});

// const deleteButtons = document.querySelectorAll(".btn-delete");
// deleteButtons.forEach((btn) => {
//   btn.addEventListener("click", function (e) {
//     e.preventDefault();
//     const modal = document.querySelector(".delete-modal");

//     modal.classList.toggle("show-modal");
//   });
// });
