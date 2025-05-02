import { TextField, RichTextField, ImageField, RichText } from '@sitecore-jss/sitecore-jss-nextjs';
import 'bootstrap/dist/css/bootstrap.min.css'; // Import Bootstrap CSS
import { useEffect } from 'react';

type FieldsCarouselDetailsFieldsFields = {
  title?: TextField;
  description?: RichTextField;
  image?: ImageField;
};

type FieldsCarouselDetailsFields = {
  id?: string;
  url?: string;
  name?: string;
  displayName?: string;
  fields?: FieldsCarouselDetailsFieldsFields;
};

type Fields = {
  CarouselDetails?: FieldsCarouselDetailsFields[];
};

const baseurl = 'https://scxp104sc.dev.local/';
type CarouselDetailsProps = {
  fields?: Fields;
};

const CarouselDetails = (props: CarouselDetailsProps): JSX.Element => {
  // Ensure Bootstrap JavaScript is loaded for carousel functionality
  useEffect(() => {
    require('bootstrap/dist/js/bootstrap.bundle.min.js');
  }, []);

  // Set the index of the active carousel item
  const activeIndex = 0;
  console.log(props);
  return (
    <div id="carouselExampleIndicators" className="carousel slide" data-bs-ride="carousel">
      <ol className="carousel-indicators">
        {props.fields?.CarouselDetails?.map((_, index) => (
          <li
            key={index}
            data-bs-target="#carouselExampleIndicators"
            data-bs-slide-to={index}
            className={index === activeIndex ? 'active' : ''}
          ></li>
        ))}
      </ol>
      <div className="carousel-inner">
        {props.fields?.CarouselDetails?.map((item, index) => (
          <div key={index} className={`carousel-item ${index === activeIndex ? 'active' : ''}`}>
            <img
              className="d-block w-100"
              src={`${baseurl}${item?.fields?.image?.value?.src}`}
              alt={`Slide ${index + 1}`}
              style={{ height: '777px', width: '100%' }}
            />
            <div className="carousel-caption d-none d-md-block">
              <h5>{item?.fields?.title?.value}</h5>
              <RichText field={item?.fields?.description} />
            </div>
          </div>
        ))}
      </div>
      <a
        className="carousel-control-prev"
        href="#carouselExampleIndicators"
        role="button"
        data-bs-slide="prev"
      >
        <span className="carousel-control-prev-icon" aria-hidden="true"></span>
        <span className="visually-hidden">Previous</span>
      </a>
      <a
        className="carousel-control-next"
        href="#carouselExampleIndicators"
        role="button"
        data-bs-slide="next"
      >
        <span className="carousel-control-next-icon" aria-hidden="true"></span>
        <span className="visually-hidden">Next</span>
      </a>
    </div>
  );
};

export default CarouselDetails;
