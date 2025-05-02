import { TextField, LinkField } from '@sitecore-jss/sitecore-jss-nextjs';
import Link from 'next/link';
import config from 'temp/config';

// Prefix public assets with a public URL to enable compatibility with Sitecore editors.
// If you're not supporting Sitecore editors, you can remove this.
const publicUrl = config.publicUrl;

type FieldsHeaderDetailsFieldsFields = {
  title?: TextField;
  link?: LinkField;
};

type FieldsHeaderDetailsFields = {
  id?: string;
  url?: string;
  name?: string;
  displayName?: string;
  fields?: FieldsHeaderDetailsFieldsFields;
};

type Fields = {
  title?: TextField;
  headerDetails?: FieldsHeaderDetailsFields[];
};
type HeaderDataProps = {
  fields?: Fields;
};

const HeaderData = (props: HeaderDataProps): JSX.Element => (
  <div className="d-flex justify-content-between align-items-center">
    <h5 className="my-0 me-md-auto fw-normal">
      <Link href="/" className="text-dark">
        <img src={`${publicUrl}/sc_logo.svg`} alt="Sitecore" />
      </Link>
    </h5>
    <nav>
      <ul className="nav">
        {props.fields?.headerDetails?.map((item) => {
          return (
            // eslint-disable-next-line react/jsx-key
            <li className="nav-item">
              <a className="nav-link text-white" href="#">
                {item.fields?.title?.value}
              </a>
            </li>
          );
        })}
      </ul>
    </nav>
  </div>
);

export default HeaderData;
