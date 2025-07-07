/**
 * Etkinlik liste öğesi model arayüzü
 */
export interface EventListItem {
  id: number;
  title: string;
  shortDescription: string;
  description: string;
  date: Date;
  eventDate: Date;
  location: string;
  categoryName: string;
  organizerName: string;
  participantCount: number;
  registrationCount: number;
  status: string;
  imageUrl?: string;
  price: number;
}

/**
 * Etkinlik detay model arayüzü
 */
export interface EventDetail {
  id: number;
  title: string;
  description: string;
  date: Date;
  location: string;
  imageUrl?: string;
  categoryName: string;
  organizerName: string;
  organizerEmail: string;
  participants: Participant[];
  createdAt: Date;
}

/**
 * Katılımcı model arayüzü
 */
export interface Participant {
  userId: number;
  fullName: string;
  email: string;
  registrationDate: Date;
}

/**
 * Etkinlik oluşturma model arayüzü
 */
export interface EventCreate {
  title: string;
  description: string;
  date: Date;
  location: string;
  imageUrl?: string;
  categoryId: number;
}