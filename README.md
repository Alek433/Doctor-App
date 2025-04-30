# 🏥 Doctor App - Role-Based Medical Management System

This is a web application designed for clinics and hospitals to manage **patients**, **doctors**, **appointments**, **medical visits**, and **billing** with **role-based access**.

---

## 🚀 Features

- 🔐 User authentication & authorization
- 🧑‍⚕️ Role-based dashboards:
  - Admin
  - Doctor
  - Patient
- 📅 Appointment booking & management
- 📋 Visit record creation with diagnosis & prescription
- 💵 Billing generation & payment flow
- 📥 Doctor-patient assignment system

---
Admin -
email: admin@hospital.com
password: Admin@123

Patient -
email: patient@hospital.com
password: Patient@123

Doctor -
email: doctor@hospital.com
password: Doctor@123

No roled user:
email: user1@hospital.com
password: User1@123

## 🔧 Setup Instructions

1. **Clone the project**

```bash
git clone https://github.com/yourusername/doctor-app.git
cd doctor-app

👤 User Roles and Access
1. Register / Login
Visit /Identity/Account/Register to create a new account.

Login via /Identity/Account/Login.

✅ Admins must approve roles Doctor to users after registration.

🧑‍⚕️ Doctor Role
After login as Doctor:

📅 View & manage your appointments

🧾 Create a Visit Record

Includes diagnosis, prescriptions, notes

💳 Add a Billing Record after visit

👀 View patient history

✅ Common Doctor Tasks
Navigate to Appointments

Click "Create Visit"

Fill in visit details

After submitting, proceed to Billing Form

Submit the bill to the patient

🧑 Patient Role
After login as Patient:

📆 Book appointments with available doctors

📋 View visit records (diagnosis, notes)

💳 View and complete billing

📩 Receive notifications

✅ Common Patient Tasks
Go to "Book Appointment"

Select doctor, time, reason

Once visited, view your Visit Summary

Check Billing → Pay or review bill

💬 Flows Overview
🔄 Assigning a Doctor
Patient only

Home → Assign Doctor

📅 Booking an Appointment
Patient

Home → Appointment Dashboard → Book Appointment → Select An Assigned Doctor & Time

📋 Creating a Visit Record
Doctor

Dashboard → Add Visit → Create Visit

💳 Billing Flow
Doctor → Patient → Admin

Doctor submits bill after visit

Patient reviews & adds insurance info

Admin verifies & approves the bill

Unnassigned user (Becoming a doctor or patient)
In home are two buttons(Becoming a Doctor or Becoming a Patient) - you choose 
