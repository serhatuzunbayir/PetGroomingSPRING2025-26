# SE 410: Pet Grooming and Veterinary Appointment Scheduler

A comprehensive, integrated system combining Web and Desktop platforms to manage clinic operations and customer appointments for veterinary and pet grooming centers.

## 👥 Project Team
**Göksun Gürel** - 20220601039 
**Eda Naz Soytürk** - 20230601063 
**Enes Deniz** - 20220601017 
**Barış Dursun** - 20220601022 

## 🛠️ 1. Functional Requirements
1. **Client and Pet Management (CRUD):** Staff must be able to create, update, and delete records for new clients and their pets within the clinic database.
2. **Pet Profiling:** Staff shall be able to define multiple pets (name, species, age, gender) for a single client and establish a one-to-many relationship between them.
3. **Customer Appointment Panel:** Customers must be able to log in via the web interface to schedule "Veterinary" or "Grooming" appointments by selecting their registered pet and an available time slot.
4. **Appointment Status Management:** Staff shall have the authority to update appointment statuses as "Pending", "Completed", or "Cancelled" for both web-initiated and manual entries.
5. **Dynamic Appointment Filtering (LINQ):** Staff must be able to perform fast filtering within the appointment list (e.g., "Today's Appointments" or "Grooming Only") using LINQ queries.
6. **Personalized Appointment Tracking (LINQ):** Customers shall be able to view their own pets' past and upcoming appointments via the web panel, filtered using LINQ.
7. **Digital Pet Record & Clinical Notes:** Upon completion of an appointment, staff must be able to add persistent medical or grooming notes (e.g., "Rabies vaccine administered") to the pet's profile.
8. **Service Billing and Payment Entry:** Staff shall be able to enter service fees and record payment data for appointments marked as "Completed".
9. **Real-Time Notification Triggers (Delegates):** The system must use C# Delegate/Event structures to trigger visual alerts on both platforms whenever a new appointment is booked or cancelled.
10. **Operational Dashboard:** Upon startup, the application shall display a summary of daily total appointments, pending tasks, and estimated daily revenue.

## ⚡ 2. Non-Functional Requirements
1. **Data Synchronization (Consistency):** An appointment booked via the Web interface must be visible on the Staff Desktop panel within a maximum of 2 seconds.
2.**Access Control (Authorization):** While customers can only access their own pet data, financial records and the full clinic calendar must be restricted to authorized staff (Role-Based Access Control).
3. **Query Performance:** LINQ search and listing operations must return results in under 2 seconds, even if the total database records exceed 5,000.
4. **User Experience (UI/UX):** The system must support keyboard shortcuts for rapid data entry by staff, while the customer interface must be fully Responsive for mobile devices.
5. **Robust Error Handling (Exception Handling):** In the event of database connection failures or invalid data entry, the system must not crash and should display user-friendly error messages instead of technical codes.

---
**Institution:** Izmir University of Economics  
**Course:** SE 410 
