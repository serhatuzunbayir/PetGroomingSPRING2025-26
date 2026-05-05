# SE 410: Pet Grooming and Veterinary Appointment Scheduler

A comprehensive, integrated system combining Web and Desktop platforms to manage clinic operations and customer appointments for veterinary and pet grooming centers.

## 👥 Project Team
**Göksun Gürel** - 20220601039 
**Eda Naz Soytürk** - 20230601063 
**Enes Deniz** - 20220601017 
**Barış Dursun** - 20220601022 

## 🛠️ 1. Functional Requirements
1. **Client and Pet Management (Desktop - CRUD):** Staff must be able to create, update, and delete records for new clients and their pets within the clinic database.
2. **Pet Profiling(Desktop):** Staff shall be able to define multiple pets (name, species, age, gender) for a single client and establish a one-to-many relationship between them.
3. **Customer Appointment Panel(Web):** Customers must be able to log in via the web interface to schedule "Veterinary" or "Grooming" appointments by selecting their registered pet and an available time slot.
4. **Appointment Status Management(Desktop):** Staff shall have the authority to update appointment statuses as "Pending", "Completed", or "Cancelled" for both web-initiated and manual entries.
5. **Dynamic Appointment Filtering (LINQ - Desktop):** Staff must be able to perform fast filtering within the appointment list (e.g., "Today's Appointments" or "Grooming Only") using LINQ queries.
6. **Personalized Appointment Tracking (LINQ - Web):** Customers shall be able to view their own pets' past and upcoming appointments via the web panel, filtered using LINQ.
7. **Digital Pet Record & Clinical Notes(Desktop):** Upon completion of an appointment, staff must be able to add persistent medical or grooming notes (e.g., "Rabies vaccine administered") to the pet's profile.
8. **Service Billing and Payment Entry(Desktop):** Staff shall be able to enter service fees and record payment data for appointments marked as "Completed".
9. **Real-Time Notification Triggers (Delegates):** The system must use C# Delegate/Event structures to trigger visual alerts on both platforms whenever a new appointment is booked or cancelled.
10. **Operational Dashboard(Desktop):** Upon startup, the application shall display a summary of daily total appointments, pending tasks, and estimated daily revenue.

## ⚡ 2. Non-Functional Requirements
1. **Data Synchronization (Consistency):** An appointment booked via the Web interface must be visible on the Staff Desktop panel within a maximum of 2 seconds.
2. **Access Control (Authorization):** While customers can only access their own pet data, financial records and the full clinic calendar must be restricted to authorized staff (Role-Based Access Control).

3. **Query Performance:** LINQ search and listing operations must return results in under 2 seconds, even if the total database records exceed 5,000.
   
4. **User Experience (UI/UX):** The system must support keyboard shortcuts for rapid data entry by staff, while the customer interface must be fully Responsive for mobile devices.

5. **Robust Error Handling (Exception Handling):** In the event of database connection failures or invalid data entry, the system must not crash and should display  error messages instead of technical codes.

## 📖 3. Integrated Project Scenario

**The system workflow begins with the receptionist logging into the Desktop application, which triggers background LINQ Aggregation queries (Sum and Count) to instantly populate the Operational Dashboard with real-time statistics, including total clients, registered pets, and financial summaries. From the centralized management panel, the receptionist initiates the Client Registration process by navigating to the "Clients" tab and performing a CRUD operation to save the owner's personal details. Once registered, the flow moves to the Pet Registration phase, where the receptionist links a specific animal to its owner, establishing a one-to-many relationship. During this stage, the LINQ-based search and filtering functionality allows the staff to list pets by owner or access specific clinical histories within seconds, ensuring an efficient and error-free registration process.

**The core of the application's interactive framework is demonstrated during the Appointment Scheduling phase. When a staff member selects a pet and defines the appointment type—such as Veterinary or Grooming—saving the record triggers the Delegate and Event mechanism in the system’s core layer. Specifically, the OnAppointmentCreated event is invoked, sending an instantaneous notification to the user interface to confirm the entry. This synchronized architecture also extends to the customer side; when a user submits a request via the Web interface, the same notification infrastructure alerts the clinic staff in real-time. This ensures that all platforms operate as a unified, interactive framework, maintaining data consistency across the desktop and web environments.

**In the final stage of the operational process, the staff utilizes LINQ Filtering to manage "Today's Appointments" and update records from "Pending" to "Completed". This action allows the receptionist to record specific health or grooming notes in the pet’s digital medical record, which customers can then view transparently through their own web dashboards. Simultaneously, the LINQ Aggregation queries re-calculate the service fees to update the clinic's financial reports. By combining practical interface steps with advanced software logic, the system provides a robust solution for managing professional veterinary and grooming care journeys.

---
**Institution:** Izmir University of Economics  
**Course:** SE 410 
